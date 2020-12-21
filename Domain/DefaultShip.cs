
namespace Domain
{
    public class DefaultShip
    {
        public int DefaultShipId { get; set; }
        public int ShipLength { get; set; }

        public int TableId { get; set; }
        public Table Table { get; set; } = null!;
        public DefaultShip Clone()
        {
            return (DefaultShip) this.MemberwiseClone();
        }
        
    }
}