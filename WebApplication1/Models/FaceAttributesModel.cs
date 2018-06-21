using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class FaceAttributesModel
    {
        public string FaceId { get; set; }
        public string Gender { get; set; }
        public string Glasses { get; set; }
        public string BlurLevel { get; set; }
        public Rectangle FaceRectangle { get; set; }
        public double Smile { get; set; }
        public double HeadPosePitch { get; set; }
        public double HeadPoseRoll { get; set; }
        public double HeadPoseYaw { get; set; }
        public double Age { get; set; }
        public double Moustache { get; set; }
        public double Beard { get; set; }
        public double Sideburns { get; set; }
        public double EmotionAnger { get; set; }
        public double EmotionContempt { get; set; }
        public double EmotionDisgust { get; set; }
        public double EmotionFear { get; set; }
        public double EmotionHappiness { get; set; }
        public double EmotionNeutral { get; set; }
        public double EmotionSadness { get; set; }
        public double EmotionSurprise { get; set; }
        public double Blur { get; set; }
        public bool HasEyeMakeup { get; set; }
        public bool HasLipMakeup { get; set; }

        public void Load(JObject data)
        {
            JObject tmp;
            JObject attribs;
            Rectangle rect;

            FaceId = data.Value<string>("faceid");
            tmp = data.Value<JObject>("faceRectangle");
            rect = new Rectangle
            {
                X = tmp.Value<int>("left"),
                Y = tmp.Value<int>("top"),
                Width = tmp.Value<int>("width"),
                Height = tmp.Value<int>("height")
            };
            FaceRectangle = rect;

            attribs = data.Value<JObject>("faceAttributes");
            Smile = attribs.Value<double>("smile");
            tmp = attribs.Value<JObject>("headPose");
            HeadPosePitch = tmp.Value<double>("pitch");
            HeadPoseRoll = tmp.Value<double>("roll");
            HeadPoseYaw = tmp.Value<double>("yaw");
            Gender = attribs.Value<string>("gender");
            Age = attribs.Value<double>("age");
            tmp = attribs.Value<JObject>("facialHair");
            Moustache = tmp.Value<double>("moustache");
            Beard = tmp.Value<double>("beard");
            Sideburns = tmp.Value<double>("sideburns");

            Glasses = attribs.Value<string>("glasses");

            tmp = attribs.Value<JObject>("emotion");
            EmotionAnger = tmp.Value<double>("anger");
            EmotionContempt = tmp.Value<double>("contempt");
            EmotionDisgust = tmp.Value<double>("disgust");
            EmotionFear = tmp.Value<double>("fear");
            EmotionHappiness = tmp.Value<double>("happiness");
            EmotionNeutral = tmp.Value<double>("neutral");
            EmotionSadness = tmp.Value<double>("sadness");
            EmotionSurprise = tmp.Value<double>("surprise");

            tmp = attribs.Value<JObject>("blur");
            BlurLevel = tmp.Value<string>("blurLevel");
            Blur = tmp.Value<double>("value");

            tmp = attribs.Value<JObject>("makeup");
            HasEyeMakeup = tmp.Value<bool>("eyeMakeup");
            HasLipMakeup = tmp.Value<bool>("lipMakeup");
        }
    }
}