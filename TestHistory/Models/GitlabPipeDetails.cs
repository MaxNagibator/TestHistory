using Newtonsoft.Json;

namespace TestHistory.Models
{
    public class DetailedStatus
    {
        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("tooltip")]
        public string Tooltip { get; set; }

        [JsonProperty("has_details")]
        public bool HasDetails { get; set; }

        [JsonProperty("details_path")]
        public string DetailsPath { get; set; }

        [JsonProperty("illustration")]
        public object Illustration { get; set; }

        [JsonProperty("favicon")]
        public string Favicon { get; set; }
    }

    public class GitlabPipeDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("iid")]
        public int Iid { get; set; }

        [JsonProperty("project_id")]
        public int ProjectId { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("before_sha")]
        public string BeforeSha { get; set; }

        [JsonProperty("tag")]
        public bool Tag { get; set; }

        [JsonProperty("yaml_errors")]
        public object YamlErrors { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }

        [JsonProperty("finished_at")]
        public DateTime FinishedAt { get; set; }

        [JsonProperty("committed_at")]
        public object CommittedAt { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("queued_duration")]
        public int QueuedDuration { get; set; }

        [JsonProperty("coverage")]
        public object Coverage { get; set; }

        [JsonProperty("detailed_status")]
        public DetailedStatus DetailedStatus { get; set; }

        [JsonProperty("name")]
        public object Name { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }
    }



}
