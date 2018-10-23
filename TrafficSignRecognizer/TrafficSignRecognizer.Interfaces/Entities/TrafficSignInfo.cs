﻿namespace TrafficSignRecognizer.Interfaces.Entities
{
    public class TrafficSignInfo
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
    }
}
