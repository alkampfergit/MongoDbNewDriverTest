using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestClasses
{
    public interface IIdentity
    {
        /// <summary>
        /// Gets the id, converted to a string.
        /// </summary>
        /// <returns>Identity converted to string</returns>
        string AsString();
    }


    public abstract class EventStoreIdentity : IIdentity
    {
        public Int64 Id { get; set; }

        public EventStoreIdentity(Int64 id)
        {
            Id = id;
        }

        public string AsString()
        {
            return ConvertAsString();
        }

        protected abstract string ConvertAsString();
    }

    public class GroupId : EventStoreIdentity
    {
        public GroupId(Int64 id) : base (id)
        {

        }
        protected override string ConvertAsString()
        {
            return "Group_" + Id;
        }
    }

    public class UserId : EventStoreIdentity
    {
        public UserId(Int64 id) : base (id)
        {

        }

        protected override string ConvertAsString()
        {
            return "User_" + Id;
        }
    }

    public static class IdentityHelper
    {
        private static Dictionary<String, Type> admittedId = new Dictionary<String, Type>()
        {
            {"Group_", typeof(GroupId) },
            {"User_", typeof(UserId) }
        };

        public static EventStoreIdentity Parse(String value)
        {
            foreach (var id in admittedId)
            {
                if (value.StartsWith(id.Key))
                    return (EventStoreIdentity) Activator.CreateInstance(id.Value, new Object[] { Int64.Parse(value.Substring(id.Key.Length)) });
            }
            throw new NotSupportedException("Identity " + value + " not supported");
        }

    }

    public class ObjectWithIdentity
    {
        public String Id { get; set; }

        public GroupId GroupId { get; set; }
    }

    public class ObjectWithArrayOfIdentities
    {
        public String Id { get; set; }

        public GroupId[] Groups { get; set; }
    }
}
