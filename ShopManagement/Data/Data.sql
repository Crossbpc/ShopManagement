create database ShopManage
go

use ShopManage
go

create table Accounts (name varchar(100) not null unique, nickname nvarchar(100), password varchar(100) not null, status bit not null default 0)
create table Tables (id int not null identity, name nvarchar(100) not null, status nvarchar(100) not null default N'Trống')
create table Categorys (id int not null identity, name nvarchar(100) not null)
create table Foods (id int not null identity, categoryid int not null, name nvarchar(100), price int not null default 0)
create table Bills (id int not null identity, tableid int not null, status bit not null default 0)
create table BillInfors (id int not null identity, billid int not null, foodid int not null, count int not null)
go

alter table bills add discount int not null default 0
alter table bills add totalmoney int not null default 0
alter table bills add Datein datetime not null default getdate()
alter table bills add Dateout datetime default null
go

alter table Accounts add primary key (name)
alter table Tables add primary key (id)
alter table Categorys add primary key (id)
alter table Foods add primary key (id)
alter table Bills add primary key (id)
alter table Billinfors add primary key (id)
go

alter table Foods add constraint Fk_food_category foreign key (categoryid) references Categorys (id)
alter table Bills add constraint Fk_bill_table foreign key (tableid) references Tables (id)
alter table Billinfors add constraint Fk_billinfor_bill foreign key (billid) references Bills(id)
alter table Billinfors add constraint Fk_billinfor_food foreign key (foodid) references Foods(id)
go


create proc sp_GetMenu @tableId int as
begin
	select  f.name, bi.count, f.price, bi.count * f.price money from bills b join billinfors bi on b.id = bi.billid join foods f on bi.foodid = f.id where b.tableid = @tableId and b.status =0
end
go

create proc sp_InsertBill @tableid int as
begin
	insert bills (tableid) values (@tableid)
end
go

create proc sp_InsertBillinfors @billid int, @foodid int, @count int as
begin	
	declare @IsExitBillInfor int
	declare @foodcount int
	declare @statusbill int
	select @statusbill = status from bills where id = @billid
	select @IsExitBillInfor = id, @foodcount = count from BillInfors where billid = @billid and foodid = @foodid and @statusbill = 0
	if(@IsExitBillInfor > 0)
	begin
		declare @newcount int = @count + @foodcount
		if(@newcount > 0)
			update billinfors set count = @newcount where billid = @billid and foodid = @foodid
		else if(@newcount <= 0)
			delete billinfors where billid = @billid and foodid = @foodid
	end
	else
	begin
		if(@count > 0)
			insert BillInfors (billid, foodid, count) values (@billid, @foodid, @count)
	end
end
go

create proc sp_CheckOut @billid int, @discount int, @finalmoney int as
begin
	update bills set status = 1, discount = @discount, totalmoney = @finalmoney, dateout = getdate() where id = @billid
end
go

create trigger tg_InsertOrUpdateBillInfor on billinfors for insert, update as
begin
	declare @billid int, @tableid int
	select @billid = billid from inserted
	select @tableid = tableid from Bills where id = @billid and status = 0
	declare @billinforcount int
	select @billinforcount = COUNT(*) from BillInfors where billid = (select id from bills where id = @billid and status = 0) 
	if(@billinforcount > 0)
		update Tables set status = N'Có người' where id = @tableid
	else
		update Tables set status = N'Trống' where id = @tableid
end
go

create trigger tg_UpdateBill on Bills for Update as
begin
	declare @tableid int
	select @tableid = tableid from inserted
	if(exists(select COUNT(id) from Bills where tableid = @tableid and status = 0))
		update Tables set status = N'Trống' where id = @tableid
end
go

create proc sp_ChangeTable @firsttableid int, @secondtableid int as
begin
	declare @firstbillid int, @secondbillid int, @isfirstbillnull int, @issecondbillnull int, @firstbillnull bit = 0, @secondbillnull bit = 0
	select @firstbillid = id from Bills where tableid = @firsttableid and status = 0
	select @secondbillid = id from Bills where tableid = @secondtableid and status = 0
	if(@firstbillid is null)
	begin
		insert bills (tableid) values (@firsttableid)
		select @firstbillid = max(id) from Bills where tableid = @firsttableid and status = 0
		set @firstbillnull = 1
	end
	select @isfirstbillnull = count(*) from BillInfors where billid = @firstbillid
	if(@secondbillid is null)
	begin
		insert bills (tableid) values (@secondtableid)
		select @secondbillid = max(id) from Bills where tableid = @secondtableid and status = 0
		set @secondbillnull = 1
	end
	select @issecondbillnull = count(*) from BillInfors where billid = @secondbillid
	select id into SecondBillInfoId from BillInfors where billid = @secondbillid
	update BillInfors set billid = @secondbillid where  billid = @firstbillid
	update BillInfors set billid = @firstbillid where id in (select * from SecondBillInfoId)
	drop table SecondBillInfoId
	if(@isfirstbillnull = 0)
		update Tables set status = N'Trống' where id = @secondtableid
	if(@issecondbillnull = 0)
		update Tables set status = N'Trống' where id = @firsttableid
	if(@firstbillnull = 1)
		delete Bills where id = @secondbillid
	if(@secondbillnull = 1)
		delete Bills where id = @firstbillid
end
go

create proc sp_Statistical @datein datetime, @dateout datetime as
begin
	select t.name [Tên bàn], b.datein [Ngày vào], b.dateout [Ngày ra], b.discount [Giảm giá %], b.totalmoney [Tổng tiền]
	from Bills b join Tables t on b.tableid = t.id
	where b.status = 1 and b.Datein >= @datein and b.Dateout <= @dateout
end
go

create proc sp_EditAccountProFile @name varchar(100), @pass varchar(100), @newpass varchar(100), @alias nvarchar(100) as
begin
	if(exists(select 1 from accounts where name = @name and password = @pass and @newpass = '2122914021714301784233128915223624866126')) -- @newpass is null
		update Accounts set nickname = @alias where name = @name	
	else if(exists(select 1 from accounts where name = @name and password = @pass and @newpass != '2122914021714301784233128915223624866126')) -- @newpass is not null
		update Accounts set nickname = @alias, password = @newpass where name = @name
end
go

create trigger tg_billinfor on billinfors for delete as
begin
	declare @billid int, @tableid int, @countbillinfor int
	declare @billidtemp int = (select billid from deleted)
	select @tableid = tableid, @billid = id from bills where id = @billidtemp and status = 0
	select @countbillinfor = COUNT(*) from BillInfors where billid = @billid
	if(@countbillinfor = 0)
	begin
		update Tables set status = N'Trống' where id = @tableid
		exec sp_CheckOut @billid, 0, 0 
	end
end
go

CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS
BEGIN
	IF @strInput IS NULL 
		RETURN @strInput 
	IF @strInput = '' 
		RETURN @strInput 
	DECLARE @RT NVARCHAR(4000), @SIGN_CHARS NCHAR(136), @UNSIGN_CHARS NCHAR (136)
	SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208)
	SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD'
	DECLARE @COUNTER int, @COUNTER1 int
	SET @COUNTER = 1
	WHILE (@COUNTER <=LEN(@strInput))
	BEGIN 
		SET @COUNTER1 = 1
		WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1)
		BEGIN 
			IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1))
			BEGIN 
				IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1)
				ELSE 
					SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER)
					BREAK
			END 
		SET @COUNTER1 = @COUNTER1 +1
		END
	SET @COUNTER = @COUNTER +1
	END 
	SET @strInput = replace(@strInput,' ','-') 
	RETURN @strInput 
END
go
