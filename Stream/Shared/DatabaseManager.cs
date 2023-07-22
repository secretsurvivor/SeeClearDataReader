using Microsoft.Data.Sqlite;

namespace Shared
{
	public class DatabaseManager
	{
		static protected SqliteConnection sqlite;

		static private void VerifyDatabaseTables()
		{
			sqlite.Open();

			var cmd = Command("select count(name) from sqlite_master where type='table' and name in ('VideoData', 'SeriesData')");

			try
			{
				object? result = cmd.ExecuteScalar();

				if (result != null)
					if ((long)result < 2)
						throw new Exception("Database was installed incorrectly, reinstall to fix error");
			}
			catch (Exception ex)
			{
				Logger.Error("DatabaseManager", "VerifyDatabaseTables", ex);
			}
			finally
			{
				sqlite.Close();
			}
		}

		static public void CreateTables()
		{
			
		}

		static DatabaseManager()
		{
			string connectionString = new SqliteConnectionStringBuilder()
			{
				Mode = SqliteOpenMode.ReadWriteCreate,
				DataSource = "sv.db"
			}.ToString();

			sqlite = new(connectionString);

			VerifyDatabaseTables();
		}

		public struct VideoData
		{
			public int VideoIndex;
			public string Title;
			public string? MetaTitle;
			public string? MetaSubtitle;
			public int Length;
			public int? SeriesFK;
			public string ThumbnailPath;
			public string LocalPath;
		}

		public struct SeriesData
		{
			public int SeriesIndex;
			public string Title;
			public bool HideChildren;
			public string? Description;
			public string ThumbnailPath;
		}

		static private SqliteCommand Command(string command)
		{
			SqliteCommand cmd = sqlite.CreateCommand();
			cmd.CommandText = command;
			return cmd;
		}

		#region Video
		/// <summary>
		/// Get all Video Data saved in Database
		/// </summary>
		/// <returns>List of Video Data Structures</returns>
		static protected List<VideoData> GetAllVideoData() 
		{
			sqlite.Open();

			var cmd = Command("select VideoIndex,Title,MetaTitle,MetaSubtitle,Length,Series,Thumbnail,LocalPath from VideoData");

			List<VideoData> datas = new();

			try
			{
				using (SqliteDataReader reader = cmd.ExecuteReader())
					if (reader.HasRows)
						while (reader.Read())
						{
							VideoData videoData = new()
							{
								VideoIndex = reader.GetInt32(0),
								Title = reader.GetString(1),
								Length = reader.GetInt32(4),
								ThumbnailPath = reader.GetString(6),
								LocalPath = reader.GetString(7)
							};

							if (!reader.IsDBNull(2))
								videoData.MetaTitle = reader.GetString(2);

							if (!reader.IsDBNull(3))
								videoData.MetaSubtitle = reader.GetString(3);

							if (!reader.IsDBNull(5))
								videoData.SeriesFK = reader.GetInt32(5);

							datas.Add(videoData);
						}
			}
			catch (Exception ex)
			{
				Logger.Error("DatabaseManager", "GetAllVideoData", ex);
			}
			finally
			{
				sqlite.Close();
			}

			return datas;
		}

		protected VideoData GetVideoData(int id)
		{
			sqlite.Open();

			var cmd = Command(@"select VideoIndex,Title,MetaTitle,MetaSubtitle,Length,Series,Thumbnail,LocalPath from VideoData where VideoIndex=$id");
			cmd.Parameters.AddWithValue("$id", id);

			Int16 error_message = 0;
			VideoData videoData = new();

			try
			{
				using(SqliteDataReader reader = cmd.ExecuteReader())
				{
					Logger.Print("HasRows", reader.HasRows);
					if (reader.HasRows)
					{
						if (reader.Read())
						{
							videoData.VideoIndex = reader.GetInt32(0);
							videoData.Title = reader.GetString(1);
							if (!reader.IsDBNull(2))
								videoData.MetaTitle = reader.GetString(2);
							if (!reader.IsDBNull(3))
								videoData.MetaSubtitle = reader.GetString(3);
							videoData.Length = reader.GetInt32(4);
							if (!reader.IsDBNull(5))
								videoData.SeriesFK = reader.GetInt32(5);
							videoData.ThumbnailPath = reader.GetString(6);
							videoData.LocalPath = reader.GetString(7);
						}
					}
					else
					{
						error_message = 1;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("DatabaseManager", "GetVideoData", ex);
			}
			finally
			{
				sqlite.Close();
			}

			if (error_message > 0)
			{
				Exception exception;

				switch (error_message)
				{
					case 1:
						exception = new Exception("VideoData with an Index of [" + id + "] does not exist");
						break;
					case 2:
						exception = new Exception("Too many records were found, one should only be found");
						break;
					default:
						error_message = -1;
						exception = new Exception("Unknown Error");
						break;
				}

				exception.Data.Add("ErrorType", error_message);

				throw exception;
			}

			return videoData;
		}
		#endregion

		#region Series
		static protected List<SeriesData> GetAllSeriesData()
		{
			sqlite.Open();

			var cmd = Command("select SeriesIndex,Title,HideChildren,Description,Thumbnail from SeriesData");

			List<SeriesData> datas = new();

			try
			{
				using (SqliteDataReader reader = cmd.ExecuteReader())
					if (reader.HasRows)
						while (reader.Read())
						{
							SeriesData seriesData = new()
							{
								SeriesIndex = reader.GetInt32(0),
								Title = reader.GetString(1),
								HideChildren = reader.GetBoolean(2),
								ThumbnailPath = reader.GetString(4)
							};

							if (!reader.IsDBNull(3))
								seriesData.Description = reader.GetString(3);

							datas.Add(seriesData);
						}
			}
			catch (Exception ex)
			{
				Logger.Error("DatabaseManager", "GetAllSeriesData", ex);
			}
			finally
			{
				sqlite.Close();
			}

			return datas;
		}

		protected SeriesData GetSeriesData(int id)
		{
			sqlite.Open();

			var cmd = Command(@"select SeriesIndex,Title,HideChildren,Description,Thumbnail from SeriesData where SeriesIndex=$id");
			cmd.Parameters.AddWithValue("$id", id);

			Int16 error_message = 0;
			SeriesData seriesData = new();

			try
			{
				using(SqliteDataReader reader = cmd.ExecuteReader())
				{
					if (reader.HasRows)
					{
						if (reader.Read())
						{
							seriesData.SeriesIndex = reader.GetInt32(0);
							seriesData.Title = reader.GetString(1);
							seriesData.HideChildren = reader.GetBoolean(2);
							if (!reader.IsDBNull(3))
								seriesData.Description = reader.GetString(3);
							seriesData.ThumbnailPath = reader.GetString(4);
						}
					}
					else
					{
						error_message = 1;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("DatabaseManager", "GetSeriesData", ex);
			}
			finally
			{
				sqlite.Close();
			}

			if (error_message > 0)
			{
				Exception exception;

				switch (error_message)
				{
					case 1:
						exception = new Exception("SeriesData with an Index of [" + id + "] does not exist");
						break;
					case 2:
						exception = new Exception("Too many records were found, one should only be found");
						break;
					default:
						error_message = -1;
						exception = new Exception("Unknown Error");
						break;
				}

				exception.Data.Add("ErrorType", error_message);

				throw exception;
			}

			return seriesData;
		}
		#endregion
	}
}
