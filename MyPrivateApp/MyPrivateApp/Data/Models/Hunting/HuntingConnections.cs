﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingConnection
    {
        [Key]
        public int HuntingConnectionId { get; set; }

        [ForeignKey("HuntingInvitation")]
        public int HuntingInvitation_FK { get; set; }
        public HuntingInvitation? HuntingInvitation { get; set; }

        [ForeignKey("HuntingTeamMembers")]
        public int HuntingTeamMembers_FK { get; set; }
        public HuntingTeamMembers? HuntingTeamMembers { get; set; }

        [ForeignKey("HuntingGuests")]
        public int HuntingGuests_FK { get; set; }
        public HuntingGuests? HuntingGuests { get; set; }
    }
}