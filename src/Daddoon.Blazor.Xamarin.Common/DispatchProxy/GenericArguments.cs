//using Mono.Cecil;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;

//public static IEnumerable<TypeReference> GetBaseTypes(
//this TypeDefinition type,
//bool includeIfaces)
//{
//    Contract.Requires(type != null);
//    Contract.Requires(
//type.IsInterface == false,
//"GetBaseTypes is not valid for interfaces");

//    var result = new List<TypeReference>();
//    var current = type;
//    var mappedFromSuperType = new List<TypeReference>();
//    var previousGenericArgsMap =
//GetGenericArgsMap(
//   type,
//   new Dictionary<string, TypeReference>(),
//   mappedFromSuperType);
//    Contract.Assert(mappedFromSuperType.Count == 0);

//    do
//    {
//        var currentBase = current.BaseType;
//        if (currentBase is GenericInstanceType)
//        {
//            previousGenericArgsMap =
//           GetGenericArgsMap(
//               current.BaseType,
//               previousGenericArgsMap,
//               mappedFromSuperType);
//            if (mappedFromSuperType.Any())
//            {
//                currentBase = ((GenericInstanceType)currentBase)
//           .ElementType.MakeGenericInstanceType(
//               previousGenericArgsMap
//                   .Select(x => x.Value)
//                   .ToArray());
//                mappedFromSuperType.Clear();
//            }
//        }
//        else
//        {
//            previousGenericArgsMap =
//       new Dictionary<string, TypeReference>();
//        }

//        result.Add(currentBase);
//        current = current.BaseType.SafeResolve(
//            string.Format(
//       CannotResolveMessage,
//       current.BaseType.FullName,
//       current.FullName));
//        if (includeIfaces)
//        {
//            result.AddRange(BuildIFaces(current, previousGenericArgsMap));
//        }
//    }
//    while (current.IsEqual(typeof(object)) == false);

//    return result;
//}

//private static IEnumerable<TypeReference> BuildIFaces(
//TypeDefinition type,
//IDictionary<string, TypeReference> genericArgsMap)
//{
//    var mappedFromSuperType = new List<TypeReference>();
//    foreach (var iface in type.Interfaces)
//    {
//        var result = iface;
//        if (iface is GenericInstanceType)
//        {
//            var map =
//       GetGenericArgsMap(
//           iface,
//           genericArgsMap,
//           mappedFromSuperType);
//            if (mappedFromSuperType.Any())
//            {
//                result = ((GenericInstanceType)iface).ElementType
//                    .MakeGenericInstanceType(
//               map.Select(x => x.Value).ToArray());
//            }
//        }

//        yield return result;
//    }
//}

//private static IDictionary<string, TypeReference> GetGenericArgsMap(
//    TypeReference type,
//    IDictionary<string, TypeReference> superTypeMap,
//    IList<TypeReference> mappedFromSuperType)
//{
//    var result = new Dictionary<string, TypeReference>();
//    if (type is GenericInstanceType == false)
//    {
//        return result;
//    }

//    var genericArgs = ((GenericInstanceType)type).GenericArguments;
//    var genericPars = ((GenericInstanceType)type)
//.ElementType.SafeResolve(CannotResolveMessage).GenericParameters;

//    /*
//     * Now genericArgs contain concrete arguments for the generic
//* parameters (genericPars).
//     *
//     * However, these concrete arguments don't necessarily have
//* to be concrete TypeReferences, these may be referencec to
//* generic parameters from super type.
//     *
//     * Example:
//     *
//     *      Consider following hierarchy:
//     *          StringMap<T> : Dictionary<string, T>
//     *
//     *          StringIntMap : StringMap<int>
//     *
//     *      What would happen if we walk up the hierarchy from StringIntMap:
//     *          -> StringIntMap
//     *              - here dont have any generic agrs or params for StringIntMap.
//     *              - but when we reesolve StringIntMap we get a
//*					reference to the base class StringMap<int>,
//     *          -> StringMap<int>
//     *              - this reference will have one generic argument
//*					System.Int32 and it's ElementType,
//     *                which is StringMap<T>, has one generic argument 'T'.
//     *              - therefore we need to remember mapping T to System.Int32
//     *              - when we resolve this class we'll get StringMap<T> and it's base
//     *              will be reference to Dictionary<string, T>
//     *          -> Dictionary<string, T>
//     *              - now *genericArgs* will be System.String and 'T'
//     *              - genericPars will be TKey and TValue from Dictionary
//* 					declaration Dictionary<TKey, TValue>
//     *              - we know that TKey is System.String and...
//     *              - because we have remembered a mapping from T to
//*					System.Int32 and now we see a mapping from TValue to T,
//     *              	we know that TValue is System.Int32, which bring us to
//*					conclusion that StringIntMap is instance of
//     *          -> Dictionary<string, int>
//     */

//    for (int i = 0; i < genericArgs.Count; i++)
//    {
//        var arg = genericArgs[i];
//        var param = genericPars[i];
//        if (arg is GenericParameter)
//        {
//            TypeReference mapping;
//            if (superTypeMap.TryGetValue(arg.Name, out mapping) == false)
//            {
//                throw new Exception(
//                    string.Format(
//                        "GetGenericArgsMap: A mapping from supertype was not found. " +
//                        "Program searched for generic argument of name {0} in supertype generic arguments map " +
//                        "as it should server as value form generic argument for generic parameter {1} in the type {2}",
//                        arg.Name,
//                        param.Name,
//                        type.FullName));
//            }

//            mappedFromSuperType.Add(mapping);
//            result.Add(param.Name, mapping);
//        }
//        else
//        {
//            result.Add(param.Name, arg);
//        }
//    }

//    return result;
//}