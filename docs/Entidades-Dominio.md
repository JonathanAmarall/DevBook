# Entidades da Camada de Domínio - LogBook

Este documento descreve as principais entidades da camada de domínio do projeto LogBook, incluindo seus campos, métodos e relacionamentos.

## Visão Geral

A camada de domínio segue os princípios do Domain-Driven Design (DDD) e contém as entidades principais do sistema, organizadas em diferentes contextos:

- **Users** - Gerenciamento de usuários
- **Entries** - Entradas/Logs do sistema
- **Notifications** - Sistema de notificações

## Classe Base: Entity

Todas as entidades principais herdam da classe base `Entity` localizada em `SharedKernel`:

```csharp
public abstract class Entity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime CreatedOnUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedOnUtc { get; protected set; }

    public void UpdateLastModifiedDate()
    {
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
```

**Campos:**
- `Id` - Identificador único (GUID)
- `CreatedOnUtc` - Data de criação
- `UpdatedOnUtc` - Data da última atualização

**Métodos:**
- `UpdateLastModifiedDate()` - Atualiza a data de modificação

---

## 1. User (Usuário)

**Localização:** `src/Domain/Users/User.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `string` | Identificador único (herdado de Entity) |
| `ExternalId` | `string?` | ID externo para autenticação OAuth (ex: GitHub) |
| `Email` | `string` | Email do usuário |
| `Username` | `string` | Nome de usuário |
| `FullName` | `string` | Nome completo |
| `PasswordHash` | `string` | Hash da senha |
| `AvatarUrl` | `Uri?` | URL do avatar |
| `Bio` | `string?` | Biografia do usuário |
| `CreatedOnUtc` | `DateTime` | Data de criação |
| `UpdatedOnUtc` | `DateTime?` | Data da última atualização |

### Métodos

- `IsExternalUser()` - Verifica se é usuário externo (baseado na presença de ExternalId)
- `UpdateMetadata(Uri avatarUrl, string name, string bio)` - Atualiza metadados do usuário

### Construtor

```csharp
public User(string email, string userName, string fullName, string passwordHash, Uri? avatarUri, string? bio, string? externalId)
```

---

## 2. Entry (Entrada/Log)

**Localização:** `src/Domain/Entries/Entry.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `string` | Identificador único (herdado de Entity) |
| `Title` | `string` | Título da entrada |
| `Description` | `string` | Descrição detalhada |
| `UserId` | `string` | ID do usuário que criou |
| `Category` | `EntryCategory` | Categoria da entrada |
| `Tags` | `List<string>` | Lista de tags |
| `Status` | `EntryStatus` | Status atual |
| `ResolvedAt` | `DateTime?` | Data de resolução |
| `CreatedOnUtc` | `DateTime` | Data de criação |
| `UpdatedOnUtc` | `DateTime?` | Data da última atualização |

### Métodos

- `MarkAsResolved()` - Marca a entrada como resolvida

### Construtor

```csharp
public Entry(string title, string description, EntryCategory category, List<string> tags, string userId)
```

**Comportamento do Construtor:**
- Se a categoria for `Bug` ou `Task`, o status inicial é `Open`
- Para outras categorias, o status é `Resolved` e `ResolvedAt` é definido

---

## 3. Notification (Notificação)

**Localização:** `src/Domain/Notifications/Notification.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `string` | Identificador único (herdado de Entity) |
| `Title` | `string` | Título da notificação |
| `EventId` | `string` | ID do evento relacionado |
| `UserId` | `string` | ID do usuário destinatário |
| `Type` | `NotificationType` | Tipo da notificação |
| `Version` | `int` | Versão da notificação (padrão: 1) |
| `Metadata` | `Dictionary<string, string>` | Metadados adicionais |
| `CreatedOnUtc` | `DateTime` | Data de criação |
| `UpdatedOnUtc` | `DateTime?` | Data da última atualização |

### Construtor

```csharp
public Notification(string title, string eventId, string userId, NotificationType type)
```

---

## 4. NotificationSchedule (Agendamento de Notificação)

**Localização:** `src/Domain/Notifications/NotificationSchedule.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `string` | Identificador único (herdado de Entity) |
| `NotificationId` | `string` | ID da notificação |
| `Frequency` | `NotificationFrequency` | Frequência de envio |
| `ScheduledTime` | `TimeOnly?` | Horário agendado |
| `DaysOfWeek` | `List<DayOfWeek>` | Dias da semana (para frequência semanal) |
| `Channels` | `List<NotificationChannel>` | Canais de entrega |
| `CreatedOnUtc` | `DateTime` | Data de criação |
| `UpdatedOnUtc` | `DateTime?` | Data da última atualização |

### Métodos

- `SetChannel(NotificationChannel channel)` - Adiciona canal de entrega

### Construtor

```csharp
public NotificationSchedule(string notificationId, NotificationFrequency frequency, TimeOnly? scheduledTime, List<DayOfWeek>? days = null)
```

**Comportamento:**
- Por padrão, inclui o canal `InApp`
- Para frequência semanal, os dias da semana são configurados

---

## 5. NotificationDelivery (Entrega de Notificação)

**Localização:** `src/Domain/Notifications/NotificationDelivery.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | `string` | Identificador único (herdado de Entity) |
| `NotificationId` | `string` | ID da notificação |
| `RecipientId` | `string` | ID do destinatário |
| `Channel` | `NotificationChannel` | Canal de entrega |
| `SentOnUtc` | `DateTime?` | Data de envio |
| `IsRead` | `bool` | Se foi lida |
| `ReadOnUtc` | `DateTime?` | Data de leitura |
| `Status` | `NotificationStatus` | Status da entrega |
| `ErrorMessage` | `string?` | Mensagem de erro (se houver) |
| `Preview` | `string` | Prévia da notificação |
| `CreatedOnUtc` | `DateTime` | Data de criação |
| `UpdatedOnUtc` | `DateTime?` | Data da última atualização |

### Métodos

- `SetErrorMessage(string error)` - Define mensagem de erro
- `MarkAsSent()` - Marca como enviada
- `MarkAsRead()` - Marca como lida

### Construtor

```csharp
public NotificationDelivery(string notificationId, string recipientId, NotificationChannel channel, string preview)
```

---

## 6. Attachment (Anexo)

**Localização:** `src/Domain/Entries/Attachment.cs`

### Campos

| Campo | Tipo | Descrição |
|-------|------|-----------|
| `FileName` | `string` | Nome do arquivo |
| `Url` | `string` | URL do arquivo |
| `UploadedAt` | `DateTime` | Data de upload |

---

## Enums

### EntryCategory
**Localização:** `src/Domain/Entries/EntryCategory.cs`

```csharp
public enum EntryCategory
{
    Bug = 0,        // Bug/Problema
    Feature = 1,    // Nova funcionalidade
    Improvement = 2, // Melhoria
    Task = 3        // Tarefa
}
```

### EntryStatus
**Localização:** `src/Domain/Entries/EntryStatus.cs`

```csharp
public enum EntryStatus
{
    Open = 0,       // Aberto
    Resolved,       // Resolvido
    InProgress      // Em andamento
}
```

### NotificationType
**Localização:** `src/Domain/Notifications/NotificationType.cs`

```csharp
public enum NotificationType
{
    Welcome = 0,    // Boas-vindas
    Reminder = 1    // Lembrete
}
```

### NotificationFrequency
**Localização:** `src/Domain/Notifications/NotificationFrequency.cs`

```csharp
public enum NotificationFrequency
{
    Once = 1,       // Uma vez
    Daily = 2,      // Diário
    Weekly = 3,     // Semanal
    Immediate = 4   // Imediato
}
```

### NotificationChannel
**Localização:** `src/Domain/Notifications/NotificationChannel.cs`

```csharp
public enum NotificationChannel
{
    InApp,          // No aplicativo
    Email,          // Email
    Push            // Push notification
}
```

### NotificationStatus
**Localização:** `src/Domain/Notifications/NotificationStatus.cs`

```csharp
public enum NotificationStatus
{
    Pending = 0,    // Pendente
    Sent = 1,       // Enviada
    Read = 2,       // Lida
    Failed = 3      // Falhou
}
```

---

## Relacionamentos

### Hierarquia de Herança
```
Entity (SharedKernel)
├── User
├── Entry
├── Notification
├── NotificationSchedule
└── NotificationDelivery
```

### Relacionamentos Principais
- **User** ↔ **Entry**: Um usuário pode ter múltiplas entradas
- **User** ↔ **Notification**: Um usuário pode receber múltiplas notificações
- **Notification** ↔ **NotificationSchedule**: Uma notificação pode ter um agendamento
- **Notification** ↔ **NotificationDelivery**: Uma notificação pode ter múltiplas entregas

---

## Princípios de Design

### Encapsulamento
- Propriedades são `private set` ou `init` para garantir imutabilidade
- Métodos públicos para modificar estado interno
- Validações de negócio encapsuladas nas entidades

### Domain-Driven Design
- Entidades representam conceitos do domínio
- Comportamento encapsulado junto com os dados
- Uso de Value Objects e Enums para representar conceitos específicos

### Clean Architecture
- Camada de domínio independente de infraestrutura
- Dependências apontam para dentro (regra de dependência)
- Entidades não dependem de frameworks externos

---

## Considerações de Implementação

### Imutabilidade
- IDs são `init` para garantir imutabilidade após criação
- Propriedades críticas são `private set` para controle de modificação

### Auditoria
- Todas as entidades herdam campos de auditoria (`CreatedOnUtc`, `UpdatedOnUtc`)
- Método `UpdateLastModifiedDate()` para atualizar automaticamente

### Validação
- Validações de domínio implementadas nos construtores e métodos
- Uso de `Result<T>` para operações que podem falhar

### Extensibilidade
- Uso de `Dictionary<string, string>` para metadados flexíveis
- Enums para tipos específicos permitem fácil extensão
