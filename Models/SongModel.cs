namespace SongModels


{
    public class JS
    {
        public Result? result { get; set; }
        public int? code { get; set; }
    }

    public class Result
    {
        public List<Track>? tracks { get; set; }
        public string? name { get; set; }
    }

    public class Track
    {
        public string? name { get; set; }
        public Album? album { get; set; }
        public string? subType { get; set; }
        public string? transName { get; set; }
    }

    public class Artist
    {
        public string? name { get; set; }
    }

    public class Album
    {
        public string? name { get; set; }
        public List<Artist>? artists { get; set; }
    }

}