/*                                                                                     *\
           _____ _____   _____   _      _              _____  _     _           
          / ____|  __ \ / ____| | |    (_)            |  __ \(_)   | |          
         | |  __| |  | | (___   | |     _ _ __   ___  | |__) |_  __| | ___ _ __ 
         | | |_ | |  | |\___ \  | |    | | '_ \ / _ \ |  _  /| |/ _` |/ _ \ '__|
         | |__| | |__| |____) | | |____| | | | |  __/ | | \ \| | (_| |  __/ |   
          \_____|_____/|_____/  |______|_|_| |_|\___| |_|  \_\_|\__,_|\___|_|   
      ˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽˽
        ©2016 Sionera Entertainment - Daniele Banovaz (daniele.banovaz@gmail.com)

        Developed as a tutorial for Game Developement Saturday #1, 2016-03-19, PN

\*                                                                                     */

using UnityEngine;

/// <summary>
/// Few extension methods used in this project
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Calculates the Z angle (in degrees) of a 2D Vector
    /// </summary>
    public static float GetZAngle(this Vector2 direction)
    {
        direction = direction.normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Convert a screen position into a world position
    /// </summary>
    public static Vector2 ScreenToWorld(this Vector2 position)
    {
        return Camera.main.ScreenToWorldPoint(position);
    }
}
