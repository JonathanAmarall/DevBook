# Diagrama de Relacionamento das Entidades - LogBook

Este documento apresenta o diagrama de relacionamento entre as entidades do domínio do projeto LogBook.

## Diagrama ER (Entity Relationship)

```mermaid
erDiagram
    %% Entidade Base
    Entity {
        string Id PK
        datetime CreatedOnUtc
        datetime UpdatedOnUtc
    }

    %% Entidades Principais
    User {
        string ExternalId
        string Email
        string Username
        string FullName
        string PasswordHash
        uri AvatarUrl
        string Bio
    }

    Entry {
        string Title
        string Description
        string UserId FK
        enum Category
        list Tags
        enum Status
        datetime ResolvedAt
    }

    Notification {
        string Title
        string EventId
        string UserId FK
        enum Type
        int Version
        dict Metadata
    }

    NotificationDelivery {
        string NotificationId FK
        string RecipientId FK
        enum Channel
        datetime SentOnUtc
        boolean IsRead
        datetime ReadOnUtc
        enum Status
        string ErrorMessage
        string Preview
    }

    NotificationSchedule {
        string NotificationId FK
        enum Frequency
        time ScheduledTime
        list DaysOfWeek
        list Channels
    }

    Attachment {
        string FileName
        string Url
        datetime UploadedAt
    }

    %% Enums
    EntryCategory {
        Bug
        Feature
        Improvement
        Task
    }

    EntryStatus {
        Open
        Resolved
        InProgress
    }

    NotificationType {
        Welcome
        Reminder
    }

    NotificationChannel {
        InApp
        Email
        Push
    }

    NotificationStatus {
        Pending
        Sent
        Read
        Failed
    }

    NotificationFrequency {
        Once
        Daily
        Weekly
        Immediate
    }

    %% Relacionamentos
    Entity ||--|| User : "herda"
    Entity ||--|| Entry : "herda"
    Entity ||--|| Notification : "herda"
    Entity ||--|| NotificationDelivery : "herda"
    Entity ||--|| NotificationSchedule : "herda"

    User ||--o{ Entry : "cria"
    User ||--o{ Notification : "recebe"
    User ||--o{ NotificationDelivery : "é destinatário"

    Entry ||--o{ Attachment : "pode ter"
    Entry }o--|| EntryCategory : "pertence a"
    Entry }o--|| EntryStatus : "tem status"

    Notification ||--|| NotificationDelivery : "gera"
    Notification ||--|| NotificationSchedule : "pode ter agendamento"
    Notification }o--|| NotificationType : "é do tipo"

    NotificationDelivery }o--|| NotificationChannel : "usa canal"
    NotificationDelivery }o--|| NotificationStatus : "tem status"

    NotificationSchedule }o--|| NotificationFrequency : "tem frequência"
    NotificationSchedule }o--o{ NotificationChannel : "usa canais"
```

## Descrição dos Relacionamentos

### 1. **User (Usuário)**
- **Entidade raiz** do sistema de usuários
- Pode ser usuário interno ou externo (via `ExternalId`)
- **Relacionamentos:**
  - 1:N com `Entry` - Um usuário pode criar múltiplas entradas
  - 1:N com `Notification` - Um usuário pode receber múltiplas notificações
  - 1:N com `NotificationDelivery` - Um usuário pode ser destinatário de múltiplas entregas

### 2. **Entry (Entrada)**
- **Entidade central** do sistema de logbook
- Representa bugs, features, melhorias ou tarefas
- **Relacionamentos:**
  - N:1 com `User` - Cada entrada pertence a um usuário
  - 1:N com `Attachment` - Uma entrada pode ter múltiplos anexos
  - N:1 com `EntryCategory` - Cada entrada tem uma categoria
  - N:1 com `EntryStatus` - Cada entrada tem um status

### 3. **Notification (Notificação)**
- **Entidade de notificação** do sistema
- Pode ser de boas-vindas ou lembretes
- **Relacionamentos:**
  - N:1 com `User` - Cada notificação pertence a um usuário
  - 1:1 com `NotificationDelivery` - Uma notificação gera uma entrega
  - 1:1 com `NotificationSchedule` - Uma notificação pode ter agendamento
  - N:1 com `NotificationType` - Cada notificação tem um tipo

### 4. **NotificationDelivery (Entrega de Notificação)**
- **Entidade de controle** de entrega de notificações
- Controla o status de leitura e entrega
- **Relacionamentos:**
  - 1:1 com `Notification` - Cada entrega corresponde a uma notificação
  - N:1 com `User` - Cada entrega tem um destinatário
  - N:1 com `NotificationChannel` - Cada entrega usa um canal
  - N:1 com `NotificationStatus` - Cada entrega tem um status

### 5. **NotificationSchedule (Agendamento de Notificação)**
- **Entidade de agendamento** de notificações
- Controla frequência e horários de envio
- **Relacionamentos:**
  - 1:1 com `Notification` - Cada agendamento pertence a uma notificação
  - N:1 com `NotificationFrequency` - Cada agendamento tem uma frequência
  - N:N com `NotificationChannel` - Um agendamento pode usar múltiplos canais

### 6. **Attachment (Anexo)**
- **Entidade de anexos** para entradas
- Representa arquivos anexados às entradas
- **Relacionamentos:**
  - N:1 com `Entry` - Cada anexo pertence a uma entrada

## Características das Entidades

### Herança
Todas as entidades principais herdam de `Entity`, que fornece:
- `Id` - Identificador único
- `CreatedOnUtc` - Data de criação
- `UpdatedOnUtc` - Data de última modificação

### Enums
O sistema utiliza vários enums para categorização:
- **EntryCategory**: Bug, Feature, Improvement, Task
- **EntryStatus**: Open, Resolved, InProgress
- **NotificationType**: Welcome, Reminder
- **NotificationChannel**: InApp, Email, Push
- **NotificationStatus**: Pending, Sent, Read, Failed
- **NotificationFrequency**: Once, Daily, Weekly, Immediate

## Padrões de Design

1. **Domain-Driven Design (DDD)**: Entidades bem definidas com comportamento encapsulado
2. **Clean Architecture**: Separação clara entre domínio e infraestrutura
3. **Value Objects**: Uso de enums para valores imutáveis
4. **Aggregate Pattern**: User como aggregate root para Entries
5. **Event Sourcing**: Notifications baseadas em eventos de domínio
