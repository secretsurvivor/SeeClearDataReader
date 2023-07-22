using Google.Protobuf;
using LocalStreamingService.Objects;
using LocalStreamingService.Objects.DataObjects;
using LocalStreamingService.Objects.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using Shared;

namespace LocalStreamingService.Controllers
{
	[ProducesResponseType(StatusCodes.Status206PartialContent)]
	[ProducesResponseType(StatusCodes.Status416RequestedRangeNotSatisfiable)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[Route("Film")]
	[Controller]
	public class FilmStreamingController : Controller
	{
		#region Controller Functions
		[Route("[action]/{token}")]
		[HttpGet]
		public IActionResult Index(string token)
		{
			if (URLLoader.Exists(token))
			{
				try
				{
					LocalResourceInfo resourceInfo = URLLoader.GetLRI(token);
					Response.ContentType = resourceInfo.ContentTypeString;

					return PhysicalFile(resourceInfo.LocalPath, resourceInfo.ContentTypeString, enableRangeProcessing: true);
				}
				catch (Exception ex)
				{
					Logger.Error("FilmStreamingController", "Index", ex);

					return BadRequest();
				}
			}
			else
			{
				return NotFound();
			}
		}

		[Route("TokenIndex/{id:int}")]
		[HttpGet]
		public IActionResult IndexRequest(int id)
		{
			string response = "";
			Response.ContentType = "text/plain";

			try
			{
				Film film = new(id);
				LocalResourceInfo info = film.BuildPathToken();

				FilmToken message = new()
				{
					VideoIndex = info.ID,
					Token = info.URLToken
				};

				Logger.Print("Film Index", info.ID);
				Logger.Print("Token", info.URLToken);

				response = MessageToString(message);
			}
			catch (Exception ex)
			{
				Logger.Error("FilmStreamingController", "IndexRequest", ex);

				response = MessageToString(ex);
			}

			return Content(response, Response.ContentType);
		}

		[Route("IndexAll")]
		[HttpGet]
		public IActionResult RequestAll()
		{
			string response = "";
			Response.ContentType = "text/plain";
			
			try
			{
				FilmInformationMessage message = new();
				List<Film> films = Film.GetAllFilms();
				Dictionary<int, SeriesMessage> seriesDict = new();

				foreach (Film film in films)
				{
					FilmMessage filmMessage = new();

					film.WriteToMessage(filmMessage); // Film Properties not including Series

					if (film.HasSeries) // Add Series if exists
					{
						if (seriesDict.ContainsKey((int)film.Series)) // Use already existing objects if exists
						{
							filmMessage.Series = seriesDict[(int)film.Series];
						}
						else
						{
							SeriesMessage seriesMessage = new();
							Series series = new((int)film.Series);

							series.WriteToMessage(ref seriesMessage);

							seriesDict.Add((int)film.Series, seriesMessage);
							filmMessage.Series = seriesMessage;
						}
					}

					message.Film.Add(filmMessage);
				}

				response = MessageToString(message);
			}
			catch (Exception ex)
			{
				Logger.Error("FilmStreamingController", "RequestAll", ex);

				response = MessageToString(ex);
			}

			return Content(response, Response.ContentType);
		}
		#endregion

		#region Support Functions
		private static IMessage<ErrorMessage> ReturnErrorMessage(Exception ex)
		{
			ErrorMessage errorMessage = new();

			if (ex.Data.Contains("ErrorType"))
			{
				switch (ex.Data["ErrorType"])
				{
					case 1:
						errorMessage.ErrorType = ErrorType.IncorrectId;
						break;
					case 2:
						errorMessage.ErrorType = ErrorType.TooManyRecords;
						break;
					case -1:
					default:
						errorMessage.ErrorType = ErrorType.UnknownError;
						break;
				}
			}
			else
			{
				errorMessage.ErrorType = ErrorType.SystemError;
			}

			errorMessage.ErrorMessage_ = ex.Message;

			return errorMessage;
		}

		private static string MessageToString(IMessage<FilmToken> message)
		{
			byte[] bytes;

			using (MemoryStream memoryStream = new())
			{
				message.WriteTo(memoryStream);
				bytes = memoryStream.ToArray();
			}

			return System.Text.Encoding.Default.GetString(bytes);
		}

		private static string MessageToString(IMessage<FilmInformationMessage> message)
		{
			byte[] bytes;

			using (MemoryStream memoryStream = new())
			{
				message.WriteTo(memoryStream);
				bytes = memoryStream.ToArray();
			}

			return System.Text.Encoding.Default.GetString(bytes);
		}

		private static string MessageToString(IMessage<ErrorMessage> message)
		{
			byte[] bytes;

			using (MemoryStream memoryStream = new())
			{
				message.WriteTo(memoryStream);
				bytes = memoryStream.ToArray();
			}

			return System.Text.Encoding.Default.GetString(bytes);
		}

		private static string MessageToString(Exception ex)
        {
			return MessageToString(ReturnErrorMessage(ex));
        }
		#endregion
	}
}
