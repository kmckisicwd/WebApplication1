using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class FacesModel
    {
        private List<FaceAttributesModel> _faces;

        public string ImagePath { get; set; }
        public IEnumerable<FaceAttributesModel> Faces { get { return _faces; } }

        [DisplayFormat(DataFormatString = "Face detections: {0}")]
        public int Detections { get { return _faces.Count; } }
        public bool IsSuccess { get; set; }

        public FacesModel()
        {
            _faces = new List<FaceAttributesModel>();
        }

        public void Load(string data)
        {
            JArray attribs;

            attribs = JArray.Parse(data);
            Load(attribs);
        }

        public void Load(JArray data)
        {
            FaceAttributesModel attrib;

            IsSuccess = false;
            _faces.Clear();
            foreach (JObject obj in data)
            {
                attrib = new FaceAttributesModel();
                attrib.Load(obj);
                _faces.Add(attrib);
            }
            IsSuccess = true;
        }
    }
}