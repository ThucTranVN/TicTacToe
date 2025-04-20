using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MyUtils
{
    public static float Distance2Point3DByXY(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
    }

    public static void OpenUrl(string url)
    {
        Debug.Log($"Url: {url}");
        Application.OpenURL($"{ url }");
    }

    public static void PhoneCall(string phone)
    {
        phone = Regex.Replace(phone, "[^0-9+]", "");

        Debug.Log($"Phone: {phone}");
        Application.OpenURL($"tel:{ phone }");
    }

    public static void OpenEmail(string email)
    {
        Debug.Log($"Email: {email}");
        Application.OpenURL($"mailto:{ email }");
    }

    public static void OpenMap(string location)
    {
        string locationUrl = location;

        if (PlatformManager.HasInstance)
        {
            switch (PlatformManager.Instance.CurrentPlatform)
            {
                case GamePlatform.Android:
                    locationUrl = CONST.ANDROID_MAP_LINK + location;
                    break;

                case GamePlatform.iOS:
                    locationUrl = CONST.IOS_MAP_LINK + WWW.EscapeURL(location);
                    break;      
            }
        }


#if UNITY_EDITOR
        locationUrl = CONST.DEFAULT_MAP_LINK + location;
#endif

        Application.OpenURL(locationUrl);
    }

    public static bool IsUIObjectsAtMouse<T>(Vector3 mousePosition, out T[] objectsAtMouse)
    {
        try
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            objectsAtMouse = results?.Where(x => x.gameObject.GetComponent<T>() != null)?.Select(y => y.gameObject.GetComponent<T>()).ToArray();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            objectsAtMouse = null;
        }

        return (objectsAtMouse?.Length ?? 0) > 0;
    }


    public static bool IsEmailValid(string email)
    {
        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        Regex regex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

        return regex.IsMatch(email);
    }

    public static void CopyTextToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    public static string GetTextFromClipboard()
    {
        return GUIUtility.systemCopyBuffer;
    }

    public static string TruncateText(string content, int maxSize = 30)
    {
        if (content.Length <= maxSize)
            return content;

        return content.Substring(0, 30).TrimEnd() + "...";
    }

    public static string EnumToString<T>(T type)
    {
        var enumType = typeof(T);
        var name = Enum.GetName(enumType, type);
        var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
        return enumMemberAttribute.Value;
    }

    public static T StringToEnum<T>(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value.ToLower() == str.ToLower()) return (T)Enum.Parse(enumType, name);
            }
        }

        return default(T);
    }

    public static long GetEpochTimeFromDateTime(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)Math.Floor(diff.TotalSeconds);
    }

    public static DateTime GetDateTimeFromEpoch(long seconds)
    {
        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return date.AddSeconds(seconds).ToLocalTime();
    }

    public static DateTime GetDateTimeFromEpochUtc(long seconds)
    {
        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return date.AddSeconds(seconds);
    }

    /// <summary>
    /// Resize raw image to fit parent
    /// </summary>
    /// <param name="image"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }

    public static Vector2 SizeToParentFitHeight(this RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;

        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }
}
