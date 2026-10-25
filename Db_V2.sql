Project voting_system {
  database_type: "SQL Server"
}

Table tbl_user {
  id bigint [pk, increment]
  email nvarchar(255) [not null, unique]
  name nvarchar(150) [not null]
  password_hash nvarchar(500)
  is_active bit [not null, default: 1]
  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint
}

Table tbl_role {
  id int [pk, increment]
  name nvarchar(50) [not null, unique]
  description nvarchar(255)
  is_active bit [not null, default: 1]
  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint
}

Table tbl_permission {
  id int [pk, increment]
  name nvarchar(100) [not null]
  code nvarchar(100) [not null, unique]
  description nvarchar(255)
  is_active bit [not null, default: 1]
  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint
}

Table tbl_user_role {
  user_id bigint [not null]
  role_id int [not null]
  assigned_at datetime2 [not null]
  assigned_by bigint

  indexes {
    (user_id, role_id) [pk]
  }
}

Table tbl_role_permission {
  role_id int [not null]
  permission_id int [not null]
  assigned_at datetime2 [not null]
  assigned_by bigint

  indexes {
    (role_id, permission_id) [pk]
  }
}

Table tbl_event {
  id bigint [pk, increment]
  name nvarchar(200) [not null]
  code nvarchar(100) [not null, unique]
  description nvarchar(1000)

  event_start_at datetime2 [not null]
  event_end_at datetime2 [not null]

  voting_start_at datetime2 [not null]
  voting_end_at datetime2 [not null]

  result_show_start_at datetime2
  result_show_end_at datetime2

  status nvarchar(30) [not null]
  is_active bit [not null, default: 1]

  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint
}

Table tbl_admin_event {
  id bigint [pk, increment]
  admin_user_id bigint [not null]
  event_id bigint [not null]

  assigned_at datetime2 [not null]
  assigned_by bigint
  is_active bit [not null, default: 1]

  indexes {
    (admin_user_id, event_id) [unique]
  }
}

Table tbl_user_event {
  id bigint [pk, increment]
  user_id bigint [not null]
  event_id bigint [not null]

  event_password_hash nvarchar(500) [not null]

  assigned_at datetime2 [not null]
  assigned_by bigint

  password_generated_at datetime2 [not null]
  password_expires_at datetime2 [not null]

  access_status nvarchar(30) [not null]

  removed_at datetime2
  removed_by bigint

  is_active bit [not null, default: 1]

  indexes {
    (user_id, event_id) [unique]
  }
}

Table tbl_project_type {
  id int [pk, increment]
  name nvarchar(50) [not null, unique]
  code nvarchar(50) [not null, unique]
  is_active bit [not null, default: 1]
}

Table tbl_project {
  id bigint [pk, increment]
  event_id bigint [not null]
  project_type_id int [not null]

  name nvarchar(200) [not null]
  description nvarchar(1000)

  is_active bit [not null, default: 1]

  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint
}

Table tbl_voting_limit {
  id bigint [pk, increment]
  user_event_id bigint [not null]
  project_type_id int [not null]

  vote_limit int [not null]
  vote_used int [not null, default: 0]

  created_at datetime2 [not null]
  created_by bigint
  modified_at datetime2
  modified_by bigint

  indexes {
    (user_event_id, project_type_id) [unique]
  }
}

Table tbl_vote {
  id bigint [pk, increment]
  user_event_id bigint [not null]
  project_id bigint [not null]
  voted_at datetime2 [not null]
}

Table tbl_result {
  id bigint [pk, increment]
  event_id bigint [not null]

  version int [not null]
  status nvarchar(30) [not null]

  reviewed_at datetime2
  reviewed_by bigint

  published_at datetime2
  published_by bigint

  created_at datetime2 [not null]
  created_by bigint

  indexes {
    (event_id, version) [unique]
  }
}

Table tbl_result_detail {
  id bigint [pk, increment]
  result_id bigint [not null]
  project_id bigint [not null]

  vote_count int [not null]
  percentage decimal(5,2) [not null]

  is_tie bit [not null, default: 0]
  is_winner bit [not null, default: 0]
}

Table tbl_result_decision {
  id bigint [pk, increment]
  result_id bigint [not null]
  project_id bigint

  decision_type nvarchar(50) [not null]
  reason nvarchar(1000)

  decided_by bigint [not null]
  decided_at datetime2 [not null]
}

Table tbl_audit_log {
  id bigint [pk, increment]

  action nvarchar(100) [not null]
  entity_type nvarchar(100) [not null]
  entity_id bigint

  performed_by bigint
  performed_at datetime2 [not null]

  old_value nvarchar(max)
  new_value nvarchar(max)

  description nvarchar(1000)
}

Ref: tbl_user_role.user_id > tbl_user.id
Ref: tbl_user_role.role_id > tbl_role.id

Ref: tbl_role_permission.role_id > tbl_role.id
Ref: tbl_role_permission.permission_id > tbl_permission.id

Ref: tbl_admin_event.admin_user_id > tbl_user.id
Ref: tbl_admin_event.event_id > tbl_event.id

Ref: tbl_user_event.user_id > tbl_user.id
Ref: tbl_user_event.event_id > tbl_event.id

Ref: tbl_project.event_id > tbl_event.id
Ref: tbl_project.project_type_id > tbl_project_type.id

Ref: tbl_voting_limit.user_event_id > tbl_user_event.id
Ref: tbl_voting_limit.project_type_id > tbl_project_type.id

Ref: tbl_vote.user_event_id > tbl_user_event.id
Ref: tbl_vote.project_id > tbl_project.id

Ref: tbl_result.event_id > tbl_event.id

Ref: tbl_result_detail.result_id > tbl_result.id
Ref: tbl_result_detail.project_id > tbl_project.id

Ref: tbl_result_decision.result_id > tbl_result.id
Ref: tbl_result_decision.project_id > tbl_project.id

Ref: tbl_audit_log.performed_by > tbl_user.id