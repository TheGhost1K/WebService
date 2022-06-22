using GemBox.Document;
using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public class ServicesViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required (ErrorMessage = "Введите полное имя")]
        [Display(Name = "Введите фамилию, имя, отчество (если есть) (Пример: Иванов Иван Иванович)")]
        public string? FullName { get; set; }

        [Required (ErrorMessage = "Введите группу")]
        [Display(Name = "Введите группу в формате МЕН-XXXXXX")]
        public string? Group { get; set; }

        [Required (ErrorMessage = "Выберите текущее направление подготовки:")]
        [Display(Name = "Выберите текущее направление подготовки:")]
        public string? Direction { get; set; }

        [Required (ErrorMessage = "Выберите направление подготовки для распределения по первому приоритету")]
        [Display(Name = "Выберите направление подготовки для распределения по первому приоритету")]
        public string? FirstProfile { get; set; }

        [Required (ErrorMessage = "Выберите направление подготовки для распределения по второму приоритету")]
        [Display(Name = "Выберите направление подготовки для распределения по второму приоритету")]
        public string? SecondProfile { get; set; }

        [Required (ErrorMessage = "Выберите направление подготовки для распределения по третьему приоритету")]
        [Display(Name = "Выберите направление подготовки для распределения по третьему приоритету")]
        public string? ThirdProfile { get; set; }
        public string Average { get; set; } = "-1";
        public string Format { get; set; } = "DOCX";
        public SaveOptions Options => FormatMappingDictionary[Format];
        public IDictionary<string, SaveOptions> FormatMappingDictionary => new Dictionary<string, SaveOptions>()
        {
            ["DOCX"] = new DocxSaveOptions()
        };
    }
}
