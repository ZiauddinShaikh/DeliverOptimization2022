﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Session
{
	public static class SessionExtension
	{
		public static void SetObject<T>(this ISession session, string key, T value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}
		public static T GetObject<T>(this ISession session, string key)
		{
			var value = session.GetString(key);
			return (string.IsNullOrEmpty(value)) ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
	}
}
