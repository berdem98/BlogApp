using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private IPostRepository _postRepository;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;

        public PostsController(IPostRepository postRepository, ITagRepository tagRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IActionResult>  Index(string tag)
        {
            var posts = _postRepository.Posts.Where(i=>i.IsActive==true);
            if(!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(p => p.Tags.Any(t=>t.Url==tag));
            }
            return View(new PostViewModel { Posts=await posts.ToListAsync()});
        }
        public async Task<IActionResult> Details(string url)
        {
            return View(await _postRepository.Posts.Include(x=>x.Tags).Include(x=>x.Comments).ThenInclude(x=>x.User).FirstOrDefaultAsync(p=>p.Url==url));
        }
        [HttpPost]
        public JsonResult AddComment(int PostId, string Text) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);
            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,      
                UserId = int.Parse(userId ?? "")
            };
            _commentRepository.CreateComment(entity);
            return Json(new
            {
                userName,
                Text,
                entity.PublishedOn,
                avatar
                
            });
            //return RedirectToRoute("post_details", new { url = Url });
            
        }
        [Authorize]
        public IActionResult Create()
        {
            //if (!User.Identity!.IsAuthenticated)
            //{
            //    return RedirectToAction("Login","User");
            //}
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Create(PostCreateViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _postRepository.CreatePost(
                    new Post
                    {
                        Title= model.Title,
                        Description=model.Description,
                        Content=model.Content,
                        Url=model.Url,
                        UserId = int.Parse(userId ?? ""),
                        PublishedOn = DateTime.Now,
                        Image="3.jpg",
                        IsActive = false
                    });
                return RedirectToAction("Index");
            }
            return View();

        }
        [Authorize]
        public async Task<IActionResult>  List() 
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);
            var post = _postRepository.Posts;
            if (string.IsNullOrEmpty(role))
            {
                post = post.Where(i => i.UserId == userId);
            }
            return View(await post.ToListAsync()); 
        }
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var post = _postRepository.Posts.FirstOrDefault(i => i.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(new PostCreateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive
            });
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(PostCreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var entityUpdate = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url
                };
                if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    entityUpdate.IsActive = model.IsActive;
                }
                _postRepository.Edit(entityUpdate);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
