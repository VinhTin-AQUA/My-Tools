# PC 04-83 Secret Code

## Secret Codes

![Logo](./assets/secret_code.webp)

## Build

```bash
dotnet publish PC0483SecretCode/PC0483SecretCode.csproj -c Release -r win-x64 -p:SelfContained=true  -o ./publish/win64
dotnet publish PC0483SecretCode/PC0483SecretCode.csproj -c Release -r linux-x64 -p:SelfContained=true  -o ./publish/linux64
```