using LocalStreamingService.Objects.Protobuf;
using Shared;

namespace LocalStreamingService.Objects.DataObjects
{
	public class Film : DatabaseManager
	{
		protected VideoData videoData;

		public string Title
		{
			get { return videoData.Title; }
		}

		public string? MetaTitle
		{
			get { return videoData.MetaTitle; }
		}

		public string? MetaSubtitle
		{
			get { return videoData.MetaSubtitle; }
		}

		public int Length
		{
			get { return videoData.Length; }
		}

		public bool HasSeries
		{
			get { return videoData.SeriesFK != null; }
		}

		public int? Series
		{
			get
			{
				if (!HasSeries)
				{
					return null;
				}
				else
				{
					return videoData.SeriesFK;
				}
			}
		}

		public Film(VideoData ddata)
		{
			videoData = ddata;
		}

		public Film(int id)
		{
			videoData = GetVideoData(id);
		}

		/// <summary>
		/// Gets all Films saved in Database in Object form
		/// </summary>
		/// <returns>List of new Film objects</returns>
		static public List<Film> GetAllFilms()
		{
			List<Film> list = new();

			foreach (VideoData data in GetAllVideoData())
			{
				list.Add(new Film(data));
			}

			return list;
		}

		public LocalResourceInfo BuildThumbnailToken()
		{
			if (URLLoader.Exists(videoData.VideoIndex * -1))
			{
				return URLLoader.GetLRI(videoData.VideoIndex * -1);
			}

			LocalResourceInfo info = URLLoader.RegisterToken(videoData.VideoIndex * -1, videoData.ThumbnailPath, ResourceType.PNG, true);
			Logger.Print("Registered Thumbnail", Title + " - " + info);

			return info;
		}

		public LocalResourceInfo BuildPathToken()
		{
			if (URLLoader.Exists(videoData.VideoIndex))
			{
				return URLLoader.GetLRI(videoData.VideoIndex);
			}

			LocalResourceInfo info = URLLoader.RegisterToken(videoData.VideoIndex, videoData.LocalPath, ResourceType.MP4);
			Logger.Print("Registered Film", Title + " - " + info);

			return info;
		}

		public void WriteToMessage(FilmMessage message)
		{
			message.VideoIndex = videoData.VideoIndex;
			message.Title = videoData.Title;

			if (videoData.MetaTitle != null)
				message.Metatitle = videoData.MetaTitle;

			if (videoData.MetaSubtitle != null)
				message.Subtitle = videoData.MetaSubtitle;

			message.ThumbnailToken = BuildThumbnailToken().URLToken;
		}
	}
}
