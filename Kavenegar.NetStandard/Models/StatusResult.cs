using Kavenegar.NetStandard.Models.Enums;

namespace Kavenegar.NetStandard.Models
{
 public class StatusResult
 {
	public long Messageid { get; set; }
	public MessageStatus Status { get; set; }
	public string Statustext { get; set; }
 }
}