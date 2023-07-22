using LocalStreamingService.Objects.Protobuf;
using Shared;

namespace LocalStreamingService.Objects.DataObjects
{
	public class Series : DatabaseManager
	{
		protected SeriesData seriesData;
		private int _id;

		protected int ID
		{
			get { return _id; }
		}

		public string Title
		{
			get { return seriesData.Title; }
		}

		public bool HideChildren
		{
			get { return seriesData.HideChildren; }
		}

		public string? Description
		{
			get { return seriesData.Description; }
		}

		public Series(SeriesData seriesData)
		{
			this.seriesData = seriesData;

			_id = (seriesData.SeriesIndex + 10000) * -1;
		}

		public Series(int id)
		{
			seriesData = GetSeriesData(id);

			_id = (seriesData.SeriesIndex + 10000) * -1;
		}

		public static List<Series> GetAllSeries()
		{
			List<Series> listSeries = new();

			foreach (SeriesData s in GetAllSeriesData())
			{
				listSeries.Add(new Series(s));
			}

			return listSeries;
		}

		public LocalResourceInfo BuildThumbnailToken()
		{
			if (URLLoader.Exists(ID))
			{
				return URLLoader.GetLRI(ID);
			}
			else
			{
				return URLLoader.RegisterToken(ID, seriesData.ThumbnailPath, ResourceType.PNG, true);
			}
		}

		public void WriteToMessage(ref SeriesMessage message)
		{
			message.SeriesIndex = seriesData.SeriesIndex;
			message.Title = seriesData.Title;
			message.HideChildren = seriesData.HideChildren;

			if (seriesData.Description != null)
				message.Description = seriesData.Description;

			message.ThumbnailToken = BuildThumbnailToken().URLToken;
		}
	}
}
