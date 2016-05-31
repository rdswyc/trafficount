
using Android.Graphics;

namespace Trafficount
{
    public static class Constants
    {
        public const string fileName = "Trafficount.xml";
        public const string xmlns = "rdswyc/Trafficount/2016";

        public const string elemRoot = "projeto";
        public const string elemCorner = "esquina";
        public const string elemCount = "contagem";
        public const string elemPed = "ped";
        public const string elemValue = "valor";

        public const string attribName = "nome";
        public const string attribDesc = "detalhe";
        public const string attribDir = "dir";
        public const string attribModal = "modal";
        public const string attribCross = "faixa";

        public static string[] Directions = { "E", "R", "D" };
        public static string[] Modals = { "carro", "moto", "ônibus", "caminhão", "bicicleta" };

        public static Color DirColor(string direction)
        {
            switch (direction)
            {
                case "E":
                    return Color.ParseColor("#1ba1e2");

                case "R":
                    return Color.ParseColor("#d80073");

                case "D":
                    return Color.ParseColor("#e3c800");

                default:
                    return Color.Gray;
            }
        }

        public static int DirIcon(string direction)
        {
            switch (direction)
            {
                case "E":
                    return Resource.Drawable.arrow_left;

                case "R":
                    return Resource.Drawable.arrow_up;

                case "D":
                    return Resource.Drawable.arrow_right;

                default:
                    return 0;
            }
        }

        public static int ModalIcon(string modal)
        {
            switch (modal)
            {
                case "carro":
                    return Resource.Drawable.modal_car;

                case "moto":
                    return Resource.Drawable.modal_motorcycle;

                case "ônibus":
                    return Resource.Drawable.modal_bus;

                case "caminhão":
                    return Resource.Drawable.modal_truck;

                case "bicicleta":
                    return Resource.Drawable.modal_bike;

                default:
                    return 0;
            }
        }
    }

    public class Corner
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Corner(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }

    public class CountItem
    {
        public string Direction { get; set; }
        public string Modal { get; set; }
        public int Value { get; set; }

        public bool IsPed { get; set; }
        public bool PedCross { get; set; }

        public Color Color { get; set; }
        public int Icon { get; set; }

        public CountItem(string direction)
        {
            Color = Color.DimGray;
            Icon = Constants.DirIcon(direction);
        }

        public CountItem(string direction, string modal, int value = 0)
        {
            Direction = direction;
            Modal = modal;
            Value = value;

            Color = Constants.DirColor(direction);
            Icon = Constants.ModalIcon(modal);
        }

        public CountItem(bool pedCross, int value = 0)
        {
            IsPed = true;
            PedCross = pedCross;
            Value = value;

            Color = Color.LightSlateGray;
            Icon = pedCross ? Resource.Drawable.ped_cross : Resource.Drawable.ped_run;
        }
    }
}