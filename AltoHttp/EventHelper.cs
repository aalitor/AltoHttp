/*
 * Created by SharpDevelop.
 * User: kafeinaltor
 * Date: 14.04.2020
 * Time: 21:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
namespace AltoHttp
{
	/// <summary>
	/// Description of EventHelper.
	/// </summary>
	public static class EventHelper
	{
		public static void Raise<T>(this EventHandler<T> ev, object sender, T e) where T:EventArgs
		{
			AsyncOperation aop = AsyncOperationManager.CreateOperation(null);
			aop.Post(delegate {
                if (ev != null)
                    ev(sender, e);
            }, null);
		}
	}
}
