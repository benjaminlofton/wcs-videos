namespace WcsVideos.Contracts
{
    using System;
    
    public class Event
    {
        public string EventId { get; set; }
		
        public string Name { get; set; }
        
		public DateTime EventDate { get; set; }
		
        public string LocationName { get; set; }
        
        public bool WsdcPointed { get; set; }
    }
}