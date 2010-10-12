using System.Linq;
using System.Linq.Expressions;

namespace Twingly.Gearman.Packets
{
    public enum PacketType
    {
        // ReSharper disable InconsistentNaming

        // Name               #     Magic  Type
        CAN_DO             =  1, // REQ    Worker
        CANT_DO            =  2, // REQ    Worker
        RESET_ABILITIES    =  3, // REQ    Worker
        PRE_SLEEP          =  4, // REQ    Worker
        // (unused)           5     -      - 
        NOOP               =  6, // RES    Worker
        SUBMIT_JOB         =  7, // REQ    Client
        JOB_CREATED        =  8, // RES    Client
        GRAB_JOB           =  9, // REQ    Worker
        NO_JOB             = 10, // RES    Worker
        JOB_ASSIGN         = 11, // RES    Worker
        WORK_STATUS        = 12, // REQ    Worker
        //                          RES    Client
        WORK_COMPLETE      = 13, // REQ    Worker
        //                          RES    Client
        WORK_FAIL          = 14, // REQ    Worker
        //                          RES    Client
        GET_STATUS         = 15, // REQ    Client
        ECHO_REQ           = 16, // REQ    Client/Worker
        ECHO_RES           = 17, // RES    Client/Worker
        SUBMIT_JOB_BG      = 18, // REQ    Client
        ERROR              = 19, // RES    Client/Worker
        STATUS_RES         = 20, // RES    Client
        SUBMIT_JOB_HIGH    = 21, // REQ    Client
        SET_CLIENT_ID      = 22, // REQ    Worker
        CAN_DO_TIMEOUT     = 23, // REQ    Worker
        ALL_YOURS          = 24, // REQ    Worker
        WORK_EXCEPTION     = 25, // REQ    Worker
        //                          RES    Client
        OPTION_REQ         = 26, // REQ    Client/Worker
        OPTION_RES         = 27, // RES    Client/Worker
        WORK_DATA          = 28, // REQ    Worker
        //                          RES    Client
        WORK_WARNING       = 29, // REQ    Worker
        //                          RES    Client
        GRAB_JOB_UNIQ      = 30, // REQ    Worker
        JOB_ASSIGN_UNIQ    = 31, // RES    Worker
        SUBMIT_JOB_HIGH_BG = 32, // REQ    Client
        SUBMIT_JOB_LOW     = 33, // REQ    Client
        SUBMIT_JOB_LOW_BG  = 34, // REQ    Client
        SUBMIT_JOB_SCHED   = 35, // REQ    Client
        SUBMIT_JOB_EPOCH   = 36, // REQ    Client

        // ReSharper restore InconsistentNaming
    }
}