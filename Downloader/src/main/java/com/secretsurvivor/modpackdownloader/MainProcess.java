/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


package com.secretsurvivor.modpackdownloader;
import com.proto.modpack.ModpackProto.Modpack;
import com.proto.modpack.ModpackProto.Modpack.Mod;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.channels.Channels;
import java.nio.channels.ReadableByteChannel;
import java.util.Scanner;
import javax.swing.SwingWorker;

/**
 *
 * @author Connor
 */
public class MainProcess {
    static class InstallWorker extends SwingWorker<Integer, Integer> {
        Modpack pack;
        public InstallWorker(Modpack pack){
            this.pack = pack;
        }
        @Override
        public Integer doInBackground() throws IOException{
            for (Mod m : pack.getModsList()){
                DownloadMod(m);
                System.out.println(m.getUrl());
                MainFrame.AddText(m.getUrl().split("/")[6]);
                MainFrame.AddPercent();
            }
            return 0;
        }
        @Override
        protected void done(){
            MainFrame.AddText("\n-- Finished installing pack --");
        }
    }
    static InstallWorker worker;
    private static Modpack modpack;
    public static int SetFile(String path) throws FileNotFoundException, IOException {
        modpack = Modpack.parseFrom(new FileInputStream(path));
        return 0;
    }
    private static void installSetup(){
        File modsDirectory = new File(SettingsFrame.minecraft_path + "//mods");
        if (!modsDirectory.exists()){
            modsDirectory.mkdir();
        }
        File[] cmods = modsDirectory.listFiles();
        if (!SettingsFrame.replace_mods){
            File pastModsDirectory = new File(SettingsFrame.minecraft_path + "//pastmods");
            if (!pastModsDirectory.exists() && cmods.length > 0){
                pastModsDirectory.mkdir();
            }
        }
        for (File f : cmods){
            if (SettingsFrame.replace_mods){
                f.delete();
            } else {
                String name = f.getName();
                f.renameTo(new File(SettingsFrame.minecraft_path + "//pastmods//" + name));
            }
        }
    }
    public static int Install() throws IOException{
        if (modpack == null){
            return 1;
        }
        installSetup();
        DownloadModpack(modpack);
        return 0;
    }
    public static int Install(File file) throws FileNotFoundException, IOException{
        try {
            modpack = Modpack.parseFrom(new FileInputStream(file));
            installSetup();
            DownloadModpack(modpack);
        } catch (IOException ex) {
            installSetup();
            Modpack t = CreateModpack("cache",file);
            DownloadModpack(t);
        }
        return 0;
    }
    public static void DownloadMod(Mod mod) throws MalformedURLException, IOException{
        URL url = new URL(mod.getUrl());
        ReadableByteChannel rbc = Channels.newChannel(url.openStream());
        try (FileOutputStream fos = new FileOutputStream(SettingsFrame.minecraft_path + "//mods//" +url.toString().split("/")[6])) {
            fos.getChannel().transferFrom(rbc, 0, Long.MAX_VALUE);
        }
    }
    public static void DownloadModpack(Modpack pack) throws IOException{
        MainFrame.SetMaxPercent(pack.getModsList().size());
        MainFrame.ClearText();
        worker = new InstallWorker(pack);
        worker.execute();
    }
    public static int CreateModpack(String name, Mod[] mods) throws FileNotFoundException, IOException{
        Modpack.Builder pack = Modpack.newBuilder();
        for (Mod m : mods){
            pack.addMods(m);
        }
        pack.build().writeTo(new FileOutputStream(name));
        return 0;
    }
    public static Modpack CreateModpack(String name,File file) throws FileNotFoundException, IOException{
        Modpack.Builder pack = Modpack.newBuilder();
        Scanner fc = new Scanner(file);
        while (fc.hasNextLine()){
            Mod.Builder mod = Mod.newBuilder();
            mod.setUrl(fc.nextLine());
            //DownloadMod(fc.nextLine());
            pack.addMods(mod);
        }
        Modpack t = pack.build();
        t.writeTo(new FileOutputStream(name));
        return t;
    }
    public static void BuildUrlModpack() throws FileNotFoundException, IOException {
        Modpack.Builder pack = Modpack.newBuilder();
        Scanner fc = new Scanner(new File(System.getProperty("user.dir")+"//pack.txt"));
        while (fc.hasNextLine()){
            Mod.Builder m = Mod.newBuilder();
            m.setUrl(fc.nextLine());
            pack.addMods(m);
        }
        pack.build().writeTo(new FileOutputStream("unnamed"));
    }

}
