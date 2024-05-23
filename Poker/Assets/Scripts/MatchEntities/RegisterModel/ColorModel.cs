using UnityEngine;

public class ColorModel 
{
    public Color Color { get; private set; }
    public ColorModel(Color color) => Color = color;

    public static byte[] Serialize(object customType)
    {
        ColorModel myType = (ColorModel)customType;
        byte[] bytes = new byte[] { (byte)myType.Color.r, (byte)myType.Color.g, (byte)myType.Color.b };
        return bytes;
    }

    public static object Deserialize(byte[] bytes)
    {
        Color color = new((float)bytes[0], (float)bytes[1], (float)bytes[2]);
        ColorModel model = new ColorModel(color);
        return model;
    }
}
