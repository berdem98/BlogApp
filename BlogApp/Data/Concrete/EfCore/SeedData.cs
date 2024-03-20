using Microsoft.EntityFrameworkCore;
using BlogApp.Entity;

namespace BlogApp.Data.Concrete.EfCore
{
	public class SeedData
	{
		public static void TestVerileriniDoldur(IApplicationBuilder app)
		{
			var context=app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();

			if(context != null)
			{
				if(context.Database.GetPendingMigrations().Any())
				{
					context.Database.Migrate();
				}
				if(!context.Tags.Any())
				{
					context.Tags.AddRange(
						new Tag { Text="C# Programlama", Url="csharp-programlama", Color=TagColors.danger },
						new Tag { Text="Python Programlama", Url = "python-programlama", Color = TagColors.secondary },
						new Tag { Text="Dart Programlama", Url = "dart-programlama", Color = TagColors.primary },
						new Tag { Text="C++ Programlama", Url = "cplusplus-programlama", Color = TagColors.info },
						new Tag { Text="Javascript Programlama", Url = "javascript-programlama", Color = TagColors.warning }
						);
					context.SaveChanges();
				}

				if (!context.Users.Any())
				{
					context.Users.AddRange(
						new User {UserName="pancarahmet",Name="Recep Ahmet Pancar",Email="pancarahmet@gmail.com", Password="123", Image="p1.jpg"},
						new User {UserName="ahmet",Name="Ahmet Demiroğlu",Email="ahmet@gmail.com",Password="123",Image = "p2.jpg" },
						new User {UserName="berk",Name="Berk Erdem",Email="berk@gmail.com",Password="123", Image= "p3.png" },
						new User { UserName="busra",Name="Büşra Şahin",Email="busra@gmail.com",Password="123", Image = "p4.jpg" },
						new User {UserName="kagan",Name="Kağan Kurt",Email="kagan@gmail.com",Password="123", Image = "p5.png" }
						);
					context.SaveChanges();
				}
				if(!context.Posts.Any())
				{
					context.Posts.AddRange(
						new Post
						{
							Title="Asp.Net Core",
							Content="C# ile birlikte Web programlama",
							Url="asp-core",
							IsActive=true,
							Image="1.jpg",
							PublishedOn=DateTime.Now.AddDays(-2),
							Tags=context.Tags.Take(3).ToList(),
							UserId=1,
							Comments= new List<Comment>
							{
								new Comment {Text="Harika",UserId=5,PublishedOn=  DateTime.Now },
								new Comment {Text="Yararlı Bir Eğitim",UserId=4,PublishedOn=  DateTime.Now.AddDays(-3) },
								new Comment {Text="Daha iyi Olabilirdi",UserId=3,PublishedOn=  DateTime.Now.AddHours(-8) },
							}
						},
						new Post
						{
							Title = "Python Programlama",
							Content = "Python ile birlikte Web programlama",
							Url = "python",
							IsActive = true,
							Image = "3.jpg",
							PublishedOn = DateTime.Now.AddDays(-10),
							Tags = context.Tags.Take(4).ToList(),
							UserId = 4
						},
						new Post
						{
							Title = "Dart",
							Content = "Dart ile birlikte Web programlama",
							Url = "dart",
							IsActive = true,
							Image = "2.jpg",
							PublishedOn = DateTime.Now.AddDays(-7),
							Tags = context.Tags.Take(1).ToList(),
							UserId = 2
						},
						new Post
						{
							Title = "Javascript",
							Content = "Javascript ile birlikte Web programlama",
							Url = "javascript",
							IsActive = true,
							Image = "3.jpg",
							PublishedOn = DateTime.Now.AddDays(-5),
							Tags = context.Tags.Take(2).ToList(),
							UserId = 3
						}
						);
					context.SaveChanges();
				}
			}
		}
	}
}
