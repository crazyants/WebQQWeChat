﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FclEx.Extensions;
using HttpAction.Core;
using HttpAction.Event;
using Newtonsoft.Json.Linq;
using WebWeChat.Im.Bean;
using WebWeChat.Im.Core;
using WebWeChat.Util;

namespace WebWeChat.Im.Action
{
    [Description("发送消息")]
    public class SendMsgAction : WebWeChatAction
    {
        private readonly MessageSent _msg;

        public SendMsgAction(IWeChatContext context, MessageSent msg, ActionEventListener listener = null) : base(context, listener)
        {
            _msg = msg;
        }

        protected override HttpRequestItem BuildRequest()
        {
            var url = string.Format(ApiUrls.SendMsg, Session.BaseUrl);
            var obj = new
            {
                Session.BaseRequest,
                Msg = _msg
            };
            var req = new HttpRequestItem(HttpMethodType.Post, url)
            {
                StringData = obj.ToJson(),
                ContentType = HttpConstants.JsonContentType
            };
            return req;
        }

        protected override Task<ActionEvent> HandleResponse(HttpResponseItem response)
        {
            var json = response.ResponseString.ToJToken();
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                return NotifyOkEventAsync();
            }
            else
            {
                throw new WeChatException(WeChatErrorCode.ResponseError, response.ResponseString);
            }
        }
    }
}
