﻿namespace ApiClients.Models
{
    public class Response<T> where T : class
    {
        public T? ResponseObject { get; set; }
        
    }
}
