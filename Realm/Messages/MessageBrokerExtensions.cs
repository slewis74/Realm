using System;
using System.Linq;
using System.Reflection;

namespace Realm.Messages
{
    public static class MessageBrokerExtensions
    {
        public static IMessageBroker SubscribeHandlersInAssembly(this IMessageBroker messageBroker, Assembly assembly)
        {
            var handlers = assembly.GetTypes().Where(t => t.GetInterfaces().Any(IsHandler));
            foreach (var handler in handlers)
            {
                var events = handler.GetInterfaces().Where(IsHandler).Select(type => type.GenericTypeArguments.Single());
                foreach (var @event in events)
                    messageBroker.Subscribe(@event, handler);
            }

            return messageBroker;
        }

        private static bool IsHandler(Type type)
        {
            var handlerInterfaceTypes = new[] { typeof(IHandle<>), typeof(IHandleAsync<>) };
            return type.IsGenericType && handlerInterfaceTypes.Contains(type.GetGenericTypeDefinition());
        }
    }
}