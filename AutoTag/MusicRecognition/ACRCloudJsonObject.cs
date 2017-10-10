using System.Collections.Generic;

namespace AutoTag.MusicRecognition
{
public class Status
{
    public string msg { get; set; }
    public int code { get; set; }
    public string version { get; set; }
}

public class ExternalIds
{
    public string isrc { get; set; }
    public string upc { get; set; }
}

public class Album
{
    public int id { get; set; }
}

public class Artist
{
    public int id { get; set; }
}

public class Genre
{
    public int id { get; set; }
}

public class Track
{
    public int id { get; set; }
}

public class Deezer
{
    public Album album { get; set; }
    public List<Artist> artists { get; set; }
    public List<Genre> genres { get; set; }
    public Track track { get; set; }
}

public class Album2
{
    public string id { get; set; }
}

public class Artist2
{
    public string id { get; set; }
}

public class Track2
{
    public string id { get; set; }
}

public class Spotify
{
    public Album2 album { get; set; }
    public List<Artist2> artists { get; set; }
    public Track2 track { get; set; }
}

public class Youtube
{
    public string vid { get; set; }
}

public class ExternalMetadata
{
    public Deezer deezer { get; set; }
    public Spotify spotify { get; set; }
    public Youtube youtube { get; set; }
}

public class Genre2
{
    public string name { get; set; }
}

public class Artist3
{
    public string name { get; set; }
}

public class Album3
{
    public string name { get; set; }
}

public class Music
{
    public ExternalIds external_ids { get; set; }
    public int play_offset_ms { get; set; }
    public ExternalMetadata external_metadata { get; set; }
    public string title { get; set; }
    public List<Genre2> genres { get; set; }
    public string release_date { get; set; }
    public List<Artist3> artists { get; set; }
    public string label { get; set; }
    public int duration_ms { get; set; }
    public Album3 album { get; set; }
    public string acrid { get; set; }
    public int result_from { get; set; }
    public int score { get; set; }
}

public class Metadata
{
    public List<Music> music { get; set; }
    public string timestamp_utc { get; set; }
}

public class ACRCloudJsonObject
{
    public Status status { get; set; }
    public Metadata metadata { get; set; }
    public int result_type { get; set; }
}
}