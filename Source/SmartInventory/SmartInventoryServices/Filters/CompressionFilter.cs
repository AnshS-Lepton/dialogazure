using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using System.Web;
using static SmartInventoryServices.Filters.CustomAuthorization;
using System.Web.Http.Controllers;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using SmartInventoryServices.WebService;

namespace SmartInventoryServices.Filters
{
    public class Compression : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response.RequestMessage.Headers.AcceptEncoding.Count > 0)
            {
                var acceptedEncoding = context.Response.RequestMessage.Headers.AcceptEncoding.First().Value;
                if (string.IsNullOrEmpty(acceptedEncoding))
                {
                    return;
                }
                if (!acceptedEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }
                context.Response.Content = new CompressedContent(context.Response.Content, acceptedEncoding);
            }
            else
            {
                return;
            }
        }

    }
    public class CompressedContent : HttpContent
    {
        private readonly string _encodingType;
        private readonly HttpContent _originalContent;
        public CompressedContent(HttpContent content, string encodingType = "gzip")
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            _originalContent = content;
            _encodingType = encodingType.ToLowerInvariant();
            foreach (var header in _originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            Headers.ContentEncoding.Add(encodingType);
        }
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Stream compressedStream = null;
            switch (_encodingType)
            {
                case "gzip":                   
                    compressedStream = new GZipStream(stream, CompressionMode.Compress, true);
                    break;
                case "deflate":
                    compressedStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    break;
                default:
                    compressedStream = stream;
                    break;
            }
            return _originalContent.CopyToAsync(compressedStream).ContinueWith(tsk =>
            {
                if (compressedStream != null)
                {
                    compressedStream.Dispose();
                }
            });
        }
    }
}