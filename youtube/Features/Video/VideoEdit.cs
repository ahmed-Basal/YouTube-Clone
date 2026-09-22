using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace youtube.viewmodels.VideoVm;

public class VideoEdit
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Display(Name = "Upload thumbnail here")]
    public IFormFile ImageUpload { get; set; }
    [Display(Name = "Upload your video here")]
    public IFormFile VideoUpload { get; set; }
    [Display(Name = "Choose the category for your video")]
    [Required(ErrorMessage = "Please choose a category")]
    public int CategoryId { get; set; }
    public IEnumerable<SelectListItem> CategoryDropdown { get; set; }
    public string ImageContentTypes { get; set; }
    public string VideoContentTypes { get; set; }
    public string ImageUrl { get; set; }
    public string VideoUrl { get; set; }

    [Display(Name = "YouTube Video URL (e.g. https://www.youtube.com/watch?v=...)")]
    public string YouTubeUrl { get; set; }
}

