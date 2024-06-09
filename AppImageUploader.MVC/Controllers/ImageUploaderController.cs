using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entidades;
using AppImageUploader.ConsumeAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AspNetCoreHero.ToastNotification.Abstractions;
namespace AppImageUploader.MVC.Controllers
{
  
    public class ImageUploaderController : Controller
    {

        private string urlApi;
        public INotyfService _notifyService { get; }
        public ImageUploaderController(IConfiguration configuration, INotyfService notifyService)
        {
            urlApi = configuration.GetValue("ApiUrlBase", "").ToString() + "/Images";
            _notifyService = notifyService;
        }

        // GET: ImageUploaderController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ImageUploaderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ImageUploaderController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Image model, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        model.Data = memoryStream.ToArray();
                        model.FileName = imageFile.FileName;
                    }
                }

                
                    ConsumirAPI<Image>.Created(urlApi, model);
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                // Manejar la excepción de alguna manera
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al procesar la imagen.");
            }
            _notifyService.Success("Imagen subida con éxito");
            return View();
        }



        // GET: ImageUploaderController/Edit/5
        public ActionResult Traer()
        {
            var images = ConsumirAPI<Image>.Read(urlApi);
           _notifyService.Success("Imagenes cargadas con éxito");
            return View(images);
        }

        // GET: ImageUploaderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ImageUploaderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageUploaderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImageUploaderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> Download(int id)
        {
            try
            {
                // Obtener la imagen desde la API utilizando el ID de forma asíncrona
                Image image = await ConsumirAPI<Image>.Read_ById_async(urlApi, id);

                // Verificar si se encontró la imagen
                if (image != null)
                {
                    // Crear un MemoryStream a partir de los datos de la imagen
                    MemoryStream memoryStream = new MemoryStream(image.Data);

                    // Establecer el tipo de contenido y el nombre del archivo para la descarga
                    Response.ContentType = "application/octet-stream";
                    Response.Headers.Add("Content-Disposition", $"attachment; filename={image.FileName}");

                    // Escribir los datos de la imagen en la respuesta de forma asíncrona
                    await memoryStream.CopyToAsync(Response.Body);

                    // Finalizar la respuesta
                    await Response.Body.FlushAsync();
                    _notifyService.Success("Imagen descargada con éxito");
                    // Retornar un resultado vacío ya que la respuesta ya se envió
                    return new EmptyResult();
                }
                else
                {
                    // Si no se encontró la imagen, redirigir a la página de detalles con un mensaje de error
                    _notifyService.Information("No se encontro la imagen");
                    return RedirectToAction("Traer", new { id = id, error = true });
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción de alguna manera
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al descargar la imagen.");
                _notifyService.Error("Ocurrió un error al descargar la imagen");
            }
            
            // Redirigir a la página de detalles
            return RedirectToAction("Traer", new { id = id });
        }

        [HttpGet]
        public async Task<ActionResult> Share(int id)
        {
            var image = await ConsumirAPI<Image>.Read_ById_async(urlApi, id);
            if (image == null)
            {
                return NotFound();
            }

            var shareUrl = Url.Action("Download", "ImageUploader", new { id = id }, Request.Scheme);
            return View((object)shareUrl);
        }



    }
}
