using UnityEngine;

public static class PuniFonts
{
    private static Font defaultFont;

    public static Font Default
    {
        get
        {
            if (defaultFont != null)
            {
                return defaultFont;
            }

            string[] fontNames =
            {
                "Apple SD Gothic Neo",
                "Arial Unicode MS",
                "Noto Sans CJK KR",
                "Malgun Gothic",
                "Arial"
            };

            defaultFont = Font.CreateDynamicFontFromOSFont(fontNames, 24);
            if (defaultFont == null)
            {
                defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            return defaultFont;
        }
    }
}
