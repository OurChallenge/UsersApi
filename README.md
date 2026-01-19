# 👤 Users API — FIAP Cloud Games

Este microsserviço é responsável pelo **cadastro, autenticação e autorização de usuários** da plataforma FIAP Cloud Games (FCG).

Ele faz parte da **Fase 2 do Tech Challenge – Pós Tech FIAP**, onde a aplicação foi refatorada para uma **arquitetura de microsserviços orientada a eventos**.

---

## 📌 Responsabilidades

- Cadastro de usuários
- Autenticação via JWT
- Autorização por perfil (User / Admin)
- Publicação de evento de criação de usuário

---

## 📬 Eventos Publicados

### 🔹 UserCreatedEvent
Evento publicado sempre que um novo usuário é cadastrado com sucesso.

**Payload exemplo:**
```json
{
  "userId": "guid",
  "email": "user@email.com",
  "createdAt": "2026-01-01T12:00:00Z"
}
