using Microsoft.AspNetCore.Mvc;
using Zufanci.Server.Service;

namespace Zufanci.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IFileUpload fileUpload;

        public ImagesController(IFileUpload fileUpload)
        {
            this.fileUpload = fileUpload;
        }

        [HttpPost("{location}")]
        public ActionResult<string> Post([FromBody] string image, string location)
        {
            try
            {
                string imagePath = fileUpload.UploadFile(image, location);
                return Ok(imagePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{location}/{imageName}")]
        public ActionResult Delete(string imageName, string location)
        {
            bool result = fileUpload.DeleteFile(imageName, location);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
