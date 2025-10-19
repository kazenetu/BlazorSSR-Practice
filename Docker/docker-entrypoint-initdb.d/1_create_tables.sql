drop table if exists m_user cascade;
create table m_user (
  unique_name TEXT
  , password TEXT not null
  , salt TEXT not null
  , totp_secrets TEXT
  , fullname TEXT not null
  , admin_role BOOLEAN default false
  , disabled BOOLEAN default true
  , version INTEGER default 1
  , constraint m_user_PKC primary key (unique_name)
) ;

drop table if exists post_master cascade;
create table post_master (
  post_cd TEXT not null
  , todohuken_kana TEXT not null
  , sikuson_kana TEXT not null
  , machi_kana TEXT
  , todohuken TEXT not null
  , sikuson TEXT not null
  , machi TEXT
  , address TEXT not null
  , address_kana TEXT not null
  , constraint post_master_PKC primary key (post_cd,todohuken_kana,sikuson_kana,machi_kana)
) ;

drop table if exists t_order cascade;
create table t_order (
  productName TEXT not null
  , unitPrice NUMERIC not null
  , qty NUMERIC not null
  , createDate TEXT not null
  , createUserId TEXT not null
  , createProgramId TEXT not null
  , updateDate TEXT
  , updateUserId TEXT
  , updateProgramId TEXT
  , version INTEGER default 1 not null
  , constraint t_order_PKC primary key (productName)
) ;

drop table if exists t_teather_forecast cascade;
create table t_teather_forecast (
  target_date TEXT not null
  , temperature_c NUMERIC not null
  , summary TEXT not null
  , createDate TEXT not null
  , createUserId TEXT not null
  , createProgramId TEXT not null
  , updateDate TEXT
  , updateUserId TEXT
  , updateProgramId TEXT
  , version INTEGER default 1 not null
  , constraint t_teather_forecast_PKC primary key (target_date)
) ;

comment on table m_user is 'ユーザーマスタ';
comment on column m_user.unique_name is 'ユニーク名:ID 英数字';
comment on column m_user.password is 'ハッシュ済パスワード';
comment on column m_user.salt is 'ハッシュソルト:パスワードのハッシュ化に使用';
comment on column m_user.totp_secrets is 'TOTPシークレットキー:ワンタイムパスワード用シークレットキー';
comment on column m_user.fullname is 'ユーザー名';
comment on column m_user.admin_role is '管理者権限';
comment on column m_user.disabled is '使廃区分';
comment on column m_user.version is 'バージョン番号:楽観ロック用';

comment on table post_master is '郵便番号マスタ';
comment on column post_master.post_cd is '郵便番号';
comment on column post_master.todohuken_kana is '都道府県名カナ';
comment on column post_master.sikuson_kana is '市区町村名カナ';
comment on column post_master.machi_kana is '町域名カナ';
comment on column post_master.todohuken is '都道府県名';
comment on column post_master.sikuson is '市区町村名';
comment on column post_master.machi is '町域名';
comment on column post_master.address is '住所:郵便番号検索時に使用';
comment on column post_master.address_kana is '住所カナ:郵便番号検索時に使用';

comment on table t_order is '注文トラン';
comment on column t_order.productName is '製品名';
comment on column t_order.unitPrice is '単価';
comment on column t_order.qty is '数量';
comment on column t_order.createDate is '登録年月日';
comment on column t_order.createUserId is '登録ユーザー';
comment on column t_order.createProgramId is '登録プログラムID';
comment on column t_order.updateDate is '更新年月日';
comment on column t_order.updateUserId is '更新ユーザー';
comment on column t_order.updateProgramId is '更新プログラムID';
comment on column t_order.version is 'バージョン番号:楽観ロック用';

comment on table t_teather_forecast is '天気予報トラン';
comment on column t_teather_forecast.target_date is '対象年月日';
comment on column t_teather_forecast.temperature_c is '気温';
comment on column t_teather_forecast.summary is '備考';
comment on column t_teather_forecast.createDate is '登録年月日';
comment on column t_teather_forecast.createUserId is '登録ユーザーID';
comment on column t_teather_forecast.createProgramId is '登録プログラムID';
comment on column t_teather_forecast.updateDate is '更新年月日';
comment on column t_teather_forecast.updateUserId is '更新プログラムID';
comment on column t_teather_forecast.updateProgramId is '更新プログラムID';
comment on column t_teather_forecast.version is 'バージョン番号:楽観ロック用';
