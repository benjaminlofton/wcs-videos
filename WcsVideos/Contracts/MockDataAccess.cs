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
					Id = "1",
					FirstName = "Maxence",
					LastName = "Martin",
					VideoIds = new string[] { "1", "4", "6" },
				}
			},
			{
				"2",
				new Dancer
				{
					Id = "2",
					FirstName = "Tatiana",
					LastName = "Mollman",
					VideoIds = new string[] { "1" },
				}
			},
			{
				"3",
				new Dancer
				{
					Id = "3",
					FirstName = "Kyle",
					LastName = "Redd",
					VideoIds = new string[] { "2", "7" },
				}
			},
			{
				"4",
				new Dancer
				{
					Id = "4",
					FirstName = "Patty",
					LastName = "Vo",
					VideoIds = new string[] { "2" },
				}
			},
			{
				"5",
				new Dancer
				{
					Id = "5",
					FirstName = "Ben",
					LastName = "Morris",
					VideoIds = new string[] { "3" },
				}
			},
			{
				"6",
				new Dancer
				{
					Id = "6",
					FirstName = "Torri",
					LastName = "Smith",
					VideoIds = new string[] { "3" },
				}
			},
			{
				"7",
				new Dancer
				{
					Id = "7",
					FirstName = "Virginie",
					LastName = "Grodin",
					VideoIds = new string[] { "4" },
				}
			},
			{
				"8",
				new Dancer
				{
					Id = "8",
					FirstName = "Jordan",
					LastName = "Frisbee",
					VideoIds = new string[] { "5" },
				}
			},
			{
				"9",
				new Dancer
				{
					Id = "9",
					FirstName = "Jessica",
					LastName = "Cox",
					VideoIds = new string[] { "5" },
				}
			},
			{
				"10",
				new Dancer
				{
					Id = "10",
					FirstName = "Tara",
					LastName = "Trafzer",
					VideoIds = new string[] { "6" }
				}
			},
			{
				"11",
				new Dancer
				{
					Id = "11",
					FirstName = "Melissa",
					LastName = "Rutz",
					VideoIds = new string[] { "7" },
				}
			},
			{
				"12",
				new Dancer
				{
					Id = "12",
					FirstName = "Diego",
					LastName = "Borges",
					VideoIds = new string[] { "12" },
				}
			},
			{
				"13",
				new Dancer
				{
					Id = "13",
					FirstName = "Jessica",
					LastName = "Pacheco",
					VideoIds = new string[] { "13" },
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
					ProviderVideoId = "vadevf89",
					Title = "Maxence and Tatiana Strictly Swing at Swingdiego",
					DancerIds = new string[] { "1", "2" }, 
				}
			},
			{
				"2",
				new Video
				{
					Id = "2",
					ProviderId = "youtube",
					ProviderVideoId = "vadevdie",
					Title = "Kyle Redd and Patty Vo Jack and Jill to Secret by Maroon 5",
					DancerIds = new string[] { "3", "4" },
				}
			},
			{
				"3",
				new Video
				{
					Id = "3",
					ProviderId = "youtube",
					ProviderVideoId = "vadioe3j",
					Title = "Ben Morris and Torri Smith at US Open",
					DancerIds = new string[] { "5", "6" },
				}
			},
			{
				"4",
				new Video
				{
					Id = "4",
					ProviderId = "youtube",
					ProviderVideoId = "vaijvelk",
					Title = "Maxence and Virginie Demo at Budafest",
					DancerIds = new string[] { "1", "7" },
				}
			},
			{
				"5",
				new Video
				{
					Id = "5",
					ProviderId = "youtube",
					ProviderVideoId = "vaiejvl",
					Title = "Jordan and Jessica dance to She Wolf",
					DancerIds = new string[] { "8", "9" },
				}
			},
			{
				"6",
				new Video
				{
					Id = "6",
					ProviderId = "youtube",
					ProviderVideoId = "v923ljvi",
					Title = "Maxence and Tara with sweet drop",
					DancerIds = new string[] { "1", "10" },
				}
			},
			{
				"7",
				new Video
				{
					Id = "7",
					ProviderId = "youtube",
					ProviderVideoId = "v2198ve",
					Title = "Kyle and Melissa at Chico",
					DancerIds = new string[] { "3", "11" }
				}
			},
			{
				"8",
				new Video
				{
					Id = "8",
					ProviderId = "youtube",
					ProviderVideoId = "vaoeivb",
					Title = "Diego Borges and Jessica Pacheco demo at Nordic Swing Dance Championships",
					DancerIds = new string[] { "12", "13" },
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