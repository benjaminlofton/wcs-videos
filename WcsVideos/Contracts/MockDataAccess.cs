using System.Collections.Generic;
using System.Linq;

namespace WcsVideos.Contracts
{
    public class MockDataAccess : IDataAccess
    {
		private readonly Dictionary<string, Dancer> dancers = new Dictionary<string, Dancer>()
		{
			{
				"1",
				new Dancer
				{
					WsdcId = "1",
					Name = "Martin, Maxence",
					VideoIdList = new string[] { "1", "4", "6" },
				}
			},
			{
				"2",
				new Dancer
				{
					WsdcId = "2",
					Name = "Mollman, Tatiana",
					VideoIdList = new string[] { "1" },
				}
			},
			{
				"3",
				new Dancer
				{
					WsdcId = "3",
					Name = "Redd, Kyle",
					VideoIdList = new string[] { "2", "7" },
				}
			},
			{
				"4",
				new Dancer
				{
					WsdcId = "4",
					Name = "Vo, Patty",
					VideoIdList = new string[] { "2" },
				}
			},
			{
				"5",
				new Dancer
				{
					WsdcId = "5",
					Name = "Morris, Ben",
					VideoIdList = new string[] { "3" },
				}
			},
			{
				"6",
				new Dancer
				{
					WsdcId = "6",
					Name = "Smith, Torri",
					VideoIdList = new string[] { "3" },
				}
			},
			{
				"7",
				new Dancer
				{
					WsdcId = "7",
					Name = "Grodin, Virginie",
					VideoIdList = new string[] { "4" },
				}
			},
			{
				"8",
				new Dancer
				{
					WsdcId = "8",
					Name = "Frisbee, Jordan",
					VideoIdList = new string[] { "5" },
				}
			},
			{
				"9",
				new Dancer
				{
					WsdcId = "9",
					Name = "Cox, Jessica",
					VideoIdList = new string[] { "5" },
				}
			},
			{
				"10",
				new Dancer
				{
					WsdcId = "10",
					Name = "Trafzer, Tara",
					VideoIdList = new string[] { "6" }
				}
			},
			{
				"11",
				new Dancer
				{
					WsdcId = "11",
					Name = "Rutz, Melissa",
					VideoIdList = new string[] { "7" },
				}
			},
			{
				"12",
				new Dancer
				{
					WsdcId = "12",
					Name = "Borges, Diego",
					VideoIdList = new string[] { "12" },
				}
			},
			{
				"13",
				new Dancer
				{
					WsdcId = "13",
					Name = "Pacheco, Jessica",
					VideoIdList = new string[] { "13" },
				}
			}
		};
		
		private readonly Dictionary<string, Video> videos = new Dictionary<string, Video>()
		{
			{
				"1",
				new Video
				{
					Id = "1",
					ProviderId = "youtube",
					ProviderVideoId = "JMZxfwqlmd0",
					Title = "Maxence and Tatiana Strictly Swing at Swingdiego",
					DancerIdList = new string[] { "1", "2" }, 
				}
			},
			{
				"2",
				new Video
				{
					Id = "2",
					ProviderId = "youtube",
					ProviderVideoId = "EqVYEiRpYRE",
					Title = "Kyle Redd and Patty Vo Jack and Jill to Secret by Maroon 5",
					DancerIdList = new string[] { "3", "4" },
				}
			},
			{
				"3",
				new Video
				{
					Id = "3",
					ProviderId = "youtube",
					ProviderVideoId = "0PznFu7w7CQ",
					Title = "Ben Morris and Torri Smith at US Open",
					DancerIdList = new string[] { "5", "6" },
				}
			},
			{
				"4",
				new Video
				{
					Id = "4",
					ProviderId = "youtube",
					ProviderVideoId = "aOD-wL-qKiw",
					Title = "Maxence and Virginie Demo at Budafest",
					DancerIdList = new string[] { "1", "7" },
				}
			},
			{
				"5",
				new Video
				{
					Id = "5",
					ProviderId = "youtube",
					ProviderVideoId = "soBV5RboKFs",
					Title = "Jordan and Jessica dance to She Wolf",
					DancerIdList = new string[] { "8", "9" },
				}
			},
			{
				"6",
				new Video
				{
					Id = "6",
					ProviderId = "youtube",
					ProviderVideoId = "D021Irk6ByI",
					Title = "Maxence and Tara with sweet drop",
					DancerIdList = new string[] { "1", "10" },
				}
			},
			{
				"7",
				new Video
				{
					Id = "7",
					ProviderId = "youtube",
					ProviderVideoId = "smoboVc3qj8",
					Title = "Kyle and Melissa at Chico",
					DancerIdList = new string[] { "3", "11" }
				}
			},
			{
				"8",
				new Video
				{
					Id = "8",
					ProviderId = "youtube",
					ProviderVideoId = "erI704Rza64",
					Title = "Diego Borges and Jessica Pacheco demo at Baltic Swing",
					DancerIdList = new string[] { "12", "13" },
				}
			}
		};
		
        public List<Video> GetTrendingVideos()
		{
			return this.videos.Values.ToList();
		}
		
		public Video GetVideoById(string id)
		{
			Video video;
			if (this.videos.TryGetValue(id, out video))
			{
				return video;
			}
			
			return null;
		}
		
		public Dancer GetDancerById(string id)
		{
			Dancer dancer;
			if (this.dancers.TryGetValue(id, out dancer))
			{
				return dancer;
			}
			
			return null;
		}
    }
}