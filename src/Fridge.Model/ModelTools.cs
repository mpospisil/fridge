using Newtonsoft.Json;

namespace Fridge.Model
{
	public static class ModelTools
	{
		public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
		{
			Formatting = Formatting.Indented,
			Culture = System.Globalization.CultureInfo.InvariantCulture,
			ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
		};

		public static Fridge DeepCopy(this Fridge src)
		{
			var jsonString = src.ToJson();
			var deepCopy = FridgeFromJson(jsonString);
			return deepCopy;
		}

		public static string ToJson(this Fridge src)
		{
			string json = JsonConvert.SerializeObject(src, SerializerSettings);
			return json;
		}

		public static Fridge FridgeFromJson(string json)
		{
			var res = JsonConvert.DeserializeObject<Fridge>(json, SerializerSettings);
			return res;
		}
	}
}
