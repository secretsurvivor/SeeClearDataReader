using System.Security.Cryptography;
using System.Text;

namespace LocalStreamingService.Objects
{
	public struct LocalResourceInfo
	{
		public int ID;
		public string URLToken;
		public bool Expire;
		public string LocalPath;
		public DateTime RegisteredTime;
		public ResourceType ContentType;
		public bool IsExpired;
		public string ContentTypeString
        {
            get
            {
                switch (ContentType)
                {
					case ResourceType.MP4:
						return "video/mp4";
					case ResourceType.PNG:
						return "image/png";
					default:
						throw new Exception("Unkown ContentType");
				}
            }
        }
		public override string ToString()
        {
			return string.Format("{0} - [{1}:{2}]", URLToken, ID, LocalPath);
        }
	}

	public enum ResourceType
	{
		MP4,
		PNG
	}

	public class URLLoader
	{
		private static int expirationCooldown = StreamingConst.URLTokenExpirationTime;
		private static Dictionary<int, LocalResourceInfo> activeIDs = new();
		private static Dictionary<string, LocalResourceInfo> activeIDsDict = new();
		private static Queue<LocalResourceInfo> activeResources = new();

		public static int TokenExpire { get { return expirationCooldown; } }

		public static LocalResourceInfo GetLRI(int id)
		{
			return activeIDs.GetValueOrDefault(id);
		}

		public static LocalResourceInfo GetLRI(string token)
		{
			return activeIDsDict.GetValueOrDefault(token);
		}

		public static bool Exists(int id)
		{
			return activeIDs.ContainsKey(id);
		}

		public static bool Exists(string token)
		{
			return activeIDsDict.ContainsKey(token);

		}

		private static string CreateNewToken(string data)
		{
			using (HashAlgorithm hash = SHA256.Create())
			{
				byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
				StringBuilder stringBuilder = new();

				for (int i = 0; i < hashBytes.Length; i++)
					stringBuilder.Append(hashBytes[i]);

				return stringBuilder.ToString();
			}
		}

		private static string CreateNewToken()
		{
			return Guid.NewGuid().ToString();
		}

		public static LocalResourceInfo RegisterToken(int id, string localPath, ResourceType type, bool expire)
		{
			if (!File.Exists(localPath))
			{
				throw new FileNotFoundException("Path [" + localPath + "] is invalid");
			}

			LocalResourceInfo newInfo = new LocalResourceInfo()
			{
				ID = id,
				URLToken = CreateNewToken(),
				Expire = expire,
				LocalPath = localPath,
				RegisteredTime = DateTime.Now,
				ContentType = type,
				IsExpired = false
			};

			activeResources.Enqueue(newInfo);
			activeIDs.Add(id, newInfo);
			activeIDsDict.Add(newInfo.URLToken.ToString(), newInfo);

			return newInfo;
		}

		public static LocalResourceInfo RegisterToken(int id, string localPath, ResourceType type)
		{
			return RegisterToken(id, localPath, type, false);
		}

		private static void ReleaseToken(LocalResourceInfo expiredToken)
		{
			activeIDs.Remove(expiredToken.ID);
			activeIDsDict.Remove(expiredToken.URLToken);
			expiredToken.IsExpired = true;
		}

		public static void ClearExpired()
		{
			for (int i = 0; i < activeResources.Count; i++)
			{
				if (DateTime.Now.Subtract(activeResources.Peek().RegisteredTime).Minutes > expirationCooldown || activeResources.Peek().Expire)
					ReleaseToken(activeResources.Dequeue());
				else break;
			}
		}
	}
}
