// See https://aka.ms/new-console-template for more information
using S3Client;

Console.WriteLine("Hello, World!");

S3Handler handler = new S3Handler();
await handler.Handle();