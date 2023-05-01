using Newtonsoft.Json;
using System;

[Serializable]
public class RecordData {
    public string region;
    public string country;
    public int regionRank;
    public int countryRank;
    public int worldRank;
    public static RecordData CreateFromJsonString(string json) {
        return JsonConvert.DeserializeObject<RecordData>(json);
    }
}

