﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol
{
    internal enum SmtpVerbs
    {
        EHLO,
        HELO,
        MAIL,
        RCPT,
        DATA,
        RSET,
        NOOP,
        QUIT,
        VRFY
    }
}
