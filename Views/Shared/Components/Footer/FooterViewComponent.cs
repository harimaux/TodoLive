using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace TodoLive.Views.Shared.Components.Footer
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _webHostEnvironment;


        public FooterViewComponent(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IViewComponentResult Invoke()
        {
            var provider = new PhysicalFileProvider(_webHostEnvironment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("Images", "TechnologiesLogos"));

            var imagesName = new List<string>();

            foreach (var img in contents)
            {
                imagesName.Add(img.Name);
            }

            return View(imagesName);
        }
    }

}
