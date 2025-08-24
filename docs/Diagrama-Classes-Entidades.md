# Diagrama de Classes das Entidades - LogBook

Este documento apresenta o diagrama de classes das entidades do domínio do projeto LogBook.

## Diagrama de Classes

```mermaid
classDiagram
    %% Classe Base
    class Entity {
        <<abstract>>
        +string Id
        +DateTime CreatedOnUtc
        +DateTime? UpdatedOnUtc
        +UpdateLastModifiedDate()
    }

    %% Entidades Principais
    class User {
        +string? ExternalId
        +string Email
        +string Username
        +string FullName
        +string PasswordHash
        +Uri? AvatarUrl
        +string? Bio
        +User(email, userName, fullName, passwordHash, avatarUri, bio, externalId)
        +IsExternalUser() bool
        +UpdateMetadata(avatarUrl, name, bio) void
    }

    class Entry {
        +string Title
        +string Description
        +string UserId
        +EntryCategory Category
        +List~string~ Tags
        +EntryStatus Status
        +DateTime? ResolvedAt
        +Entry(title, description, category, tags, userId)
        +MarkAsResolved() void
    }

    class Notification {
        +string Title
        +string EventId
        +string UserId
        +NotificationType Type
        +int Version
        +Dictionary~string,string~ Metadata
        +Notification(title, eventId, userId, type)
    }

    class NotificationDelivery {
        +string NotificationId
        +string RecipientId
        +NotificationChannel Channel
        +DateTime? SentOnUtc
        +bool IsRead
        +DateTime? ReadOnUtc
        +NotificationStatus Status
        +string? ErrorMessage
        +string Preview
        +NotificationDelivery(notificationId, recipientId, channel, preview)
        +SetErrorMessage(error) void
        +MarkAsSent() Result
        +MarkAsRead() Result
    }

    class NotificationSchedule {
        +string NotificationId
        +NotificationFrequency Frequency
        +TimeOnly? ScheduledTime
        +List~DayOfWeek~ DaysOfWeek
        +List~NotificationChannel~ Channels
        +NotificationSchedule(notificationId, frequency, scheduledTime, days)
        +SetChannel(channel) void
    }

    class Attachment {
        +string FileName
        +string Url
        +DateTime UploadedAt
    }

    %% Enums
    class EntryCategory {
        <<enumeration>>
        Bug
        Feature
        Improvement
        Task
    }

    class EntryStatus {
        <<enumeration>>
        Open
        Resolved
        InProgress
    }

    class NotificationType {
        <<enumeration>>
        Welcome
        Reminder
    }

    class NotificationChannel {
        <<enumeration>>
        InApp
        Email
        Push
    }

    class NotificationStatus {
        <<enumeration>>
        Pending
        Sent
        Read
        Failed
    }

    class NotificationFrequency {
        <<enumeration>>
        Once
        Daily
        Weekly
        Immediate
    }

    %% Relacionamentos de Herança
    Entity <|-- User
    Entity <|-- Entry
    Entity <|-- Notification
    Entity <|-- NotificationDelivery
    Entity <|-- NotificationSchedule

    %% Relacionamentos de Associação
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

## Análise das Entidades

### 1. **Entity (Classe Base)**
- **Propósito**: Classe base abstrata para todas as entidades
- **Responsabilidades**:
  - Fornecer identificador único (`Id`)
  - Controlar timestamps de criação e modificação
  - Fornecer método para atualizar data de modificação

### 2. **User (Usuário)**
- **Propósito**: Representa usuários do sistema
- **Responsabilidades**:
  - Gerenciar informações de perfil do usuário
  - Suportar autenticação interna e externa
  - Permitir atualização de metadados
- **Comportamentos**:
  - `IsExternalUser()`: Verifica se é usuário externo
  - `UpdateMetadata()`: Atualiza informações do perfil

### 3. **Entry (Entrada)**
- **Propósito**: Entidade central do logbook
- **Responsabilidades**:
  - Representar bugs, features, melhorias ou tarefas
  - Gerenciar status e resolução
  - Armazenar tags e categorização
- **Comportamentos**:
  - `MarkAsResolved()`: Marca entrada como resolvida

### 4. **Notification (Notificação)**
- **Propósito**: Representar notificações do sistema
- **Responsabilidades**:
  - Armazenar informações da notificação
  - Suportar versionamento
  - Manter metadados flexíveis
- **Características**:
  - Imutável após criação
  - Suporte a diferentes tipos

### 5. **NotificationDelivery (Entrega de Notificação)**
- **Propósito**: Controlar entrega e leitura de notificações
- **Responsabilidades**:
  - Rastrear status de entrega
  - Controlar leitura de notificações
  - Gerenciar erros de entrega
- **Comportamentos**:
  - `MarkAsSent()`: Marca como enviada
  - `MarkAsRead()`: Marca como lida
  - `SetErrorMessage()`: Define mensagem de erro

### 6. **NotificationSchedule (Agendamento)**
- **Propósito**: Gerenciar agendamento de notificações
- **Responsabilidades**:
  - Definir frequência de envio
  - Configurar horários e dias da semana
  - Suportar múltiplos canais
- **Comportamentos**:
  - `SetChannel()`: Adiciona canal de entrega

### 7. **Attachment (Anexo)**
- **Propósito**: Representar arquivos anexados
- **Responsabilidades**:
  - Armazenar informações do arquivo
  - Manter URL de acesso
  - Controlar timestamp de upload

## Padrões de Design Identificados

### 1. **Domain-Driven Design (DDD)**
- Entidades ricas com comportamento encapsulado
- Agregados bem definidos (User como aggregate root)
- Value objects (enums para categorização)

### 2. **Clean Architecture**
- Separação clara entre domínio e infraestrutura
- Entidades independentes de frameworks
- Regras de negócio centralizadas

### 3. **Immutabilidade**
- Propriedades com `init` ou `private set`
- Métodos para alteração de estado
- Proteção contra modificações diretas

### 4. **Result Pattern**
- Uso de `Result` para operações que podem falhar
- Tratamento explícito de erros
- Validação de regras de negócio

### 5. **Factory Pattern**
- Construtores que garantem estado válido
- Inicialização adequada de propriedades
- Validação durante criação

## Considerações de Design

### Pontos Fortes
1. **Encapsulamento**: Comportamento encapsulado nas entidades
2. **Imutabilidade**: Proteção contra modificações indevidas
3. **Flexibilidade**: Suporte a diferentes tipos de usuário e notificação
4. **Rastreabilidade**: Controle completo de timestamps
5. **Extensibilidade**: Estrutura preparada para crescimento

### Áreas de Melhoria
1. **Validação**: Poderia ter validações mais robustas
2. **Eventos**: Implementação de domain events
3. **Auditoria**: Logs de mudanças de estado
4. **Cache**: Estratégias de cache para consultas frequentes
