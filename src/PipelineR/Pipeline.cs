﻿using System;

namespace PipelineR
{
    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
    {
        private IRequestHandler<TContext, TRequest> _requestHandler;
        private IRequestHandler<TContext, TRequest> _finallyRequestHandler;

        public static Pipeline<TContext, TRequest> Build()
        {
            return new Pipeline<TContext, TRequest>();
        }

        public Pipeline<TContext, TRequest> AddNext(IRequestHandler<TContext, TRequest> requestHandler)
        {
            if (this._requestHandler == null)
            {
                this._requestHandler = requestHandler;
            }
            else
            {
                this.GetLastRequestHandler(this._requestHandler).NextRequestHandler = requestHandler;
            }

            return this;
        }
        public Pipeline<TContext, TRequest> AddFinally(IRequestHandler<TContext, TRequest> requestHandler)
        {
            _finallyRequestHandler = requestHandler;

            return this;
        }



        public RequestHandlerResult Execute(TRequest request)
        {
            if(this._requestHandler==null)
                throw new ArgumentNullException("No started handlers");

            this._requestHandler.Context.Request = request;

            var result= this._requestHandler.HandleRequest(request);

            if (this._finallyRequestHandler != null)
                result = this._finallyRequestHandler.HandleRequest(request);

            return result;
        }

        private IRequestHandler<TContext, TRequest> GetLastRequestHandler(
            IRequestHandler<TContext, TRequest> requestHandler)
        {
            if (requestHandler.NextRequestHandler != null)
                return GetLastRequestHandler(requestHandler.NextRequestHandler);
            
            return requestHandler;
        }
    }

    public interface IPipeline<TContext, in TRequest> where TContext : BaseContext
    {
        RequestHandlerResult Execute(TRequest request);
    }
}