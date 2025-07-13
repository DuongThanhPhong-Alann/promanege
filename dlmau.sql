create database promanege
go
use promanege
go


-- Bảng Vai trò
CREATE TABLE VaiTro (
    ID NVARCHAR(12) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL UNIQUE
);

-- Bảng Người dùng
CREATE TABLE NguoiDung (
    ID NVARCHAR(12) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    SoDienThoai NVARCHAR(20),
    MatKhau NVARCHAR(255) NOT NULL
);

-- Bảng Vai trò người dùng
CREATE TABLE VaiTroNguoiDung (
    NguoiDungID NVARCHAR(12),
    VaiTroID NVARCHAR(12),
    PRIMARY KEY (NguoiDungID, VaiTroID),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(ID),
    FOREIGN KEY (VaiTroID) REFERENCES VaiTro(ID)
);

-- Bảng Nhóm công việc
CREATE TABLE NhomCongViec (
    ID NVARCHAR(12) PRIMARY KEY,
    TenNhom NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255),
    MucTieu NVARCHAR(255),
    NgayBatDau DATE,
    NgayKetThuc DATE,
    NguoiTaoID NVARCHAR(12),
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (NguoiTaoID) REFERENCES NguoiDung(ID)
);

-- Bảng Thành viên nhóm
CREATE TABLE ThanhVienNhom (
    NhomID NVARCHAR(12),
    NguoiDungID NVARCHAR(12),
    PRIMARY KEY (NhomID, NguoiDungID),
    FOREIGN KEY (NhomID) REFERENCES NhomCongViec(ID),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(ID)
);

-- Bảng Công việc
CREATE TABLE CongViec (
    ID NVARCHAR(12) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    LoaiCongViec NVARCHAR(10) CHECK (LoaiCongViec IN ('CaNhan', 'Nhom')),
    NguoiTaoID NVARCHAR(12) NOT NULL,
    NhomID NVARCHAR(12), -- NULL nếu là cá nhân
    FOREIGN KEY (NguoiTaoID) REFERENCES NguoiDung(ID),
    FOREIGN KEY (NhomID) REFERENCES NhomCongViec(ID)
);

-- Bảng Chi tiết công việc
CREATE TABLE ChiTietCongViec (
    CongViecID NVARCHAR(12) PRIMARY KEY,
    MoTa NVARCHAR(MAX),
    NgayBatDau DATE,
    NgayKetThuc DATE,
    FOREIGN KEY (CongViecID) REFERENCES CongViec(ID)
);

-- Bảng Phân công công việc
CREATE TABLE PhanCongCongViec (
    CongViecID NVARCHAR(12),
    NguoiDungID NVARCHAR(12),
    NgayPhanCong DATE DEFAULT GETDATE(),
    PRIMARY KEY (CongViecID, NguoiDungID),
    FOREIGN KEY (CongViecID) REFERENCES CongViec(ID),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(ID)
);

-- Bảng Tiến độ công việc tổng thể
CREATE TABLE TienDoCongViec (
    CongViecID NVARCHAR(12) PRIMARY KEY,
    PhanTramHoanThanh INT CHECK (PhanTramHoanThanh BETWEEN 0 AND 100),
    SoNgayDaLam INT,
    FOREIGN KEY (CongViecID) REFERENCES CongViec(ID)
);

-- Bảng Tiến độ thành viên trong công việc nhóm
CREATE TABLE TienDoThanhVien (
    CongViecID NVARCHAR(12),
    NguoiDungID NVARCHAR(12),
    PhanTramHoanThanh INT CHECK (PhanTramHoanThanh BETWEEN 0 AND 100),
    SoNgayDaLam INT,
    PRIMARY KEY (CongViecID, NguoiDungID),
    FOREIGN KEY (CongViecID) REFERENCES CongViec(ID),
    FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(ID)
);

-- Bảng Hình ảnh tách riêng
CREATE TABLE HinhAnh (
    ID NVARCHAR(12) PRIMARY KEY,
    DuongDan NVARCHAR(255) NOT NULL,
    LoaiDoiTuong NVARCHAR(20) CHECK (LoaiDoiTuong IN ('NguoiDung', 'Nhom', 'CongViec')),
    DoiTuongID NVARCHAR(12) NOT NULL
    -- Không có FK cứng vì DoiTuongID có thể là NguoiDungID, NhomID hoặc CongViecID
);


-----------------------------------------------------------------------------------------

-- Vai trò
INSERT INTO VaiTro (ID, TenVaiTro) VALUES 
(N'VT001', N'NguoiDung'),
(N'VT002', N'QuanLy');

-- Người dùng
INSERT INTO NguoiDung (ID, TenDangNhap, Email, SoDienThoai, MatKhau) VALUES 
(N'ND001', N'thanhphong', N'phong@example.com', N'0909123456', N'hashed_password_1'),
(N'ND002', N'minhthu', N'thu@example.com', N'0987654321', N'hashed_password_2');

-- Vai trò người dùng
INSERT INTO VaiTroNguoiDung (NguoiDungID, VaiTroID) VALUES 
(N'ND001', N'VT002'),
(N'ND002', N'VT001');

-- Nhóm công việc
INSERT INTO NhomCongViec (ID, TenNhom, MoTa, MucTieu, NgayBatDau, NgayKetThuc, NguoiTaoID) VALUES 
(N'NH001', N'Dự án Web QLCT', N'Xây dựng hệ thống quản lý công việc', N'Hoàn thành MVP', '2025-07-01', '2025-08-15', N'ND001'),
(N'NH002', N'Nghiên cứu AI', N'Tìm hiểu và thử nghiệm mô hình GPT', N'Viết báo cáo cuối kỳ', '2025-06-20', '2025-08-30', N'ND002');

-- Thành viên nhóm
INSERT INTO ThanhVienNhom (NhomID, NguoiDungID) VALUES 
(N'NH001', N'ND001'),
(N'NH001', N'ND002');

-- Công việc
INSERT INTO CongViec (ID, TieuDe, LoaiCongViec, NguoiTaoID, NhomID) VALUES 
(N'CV001', N'Thiết kế UI', N'Nhom', N'ND001', N'NH001'),
(N'CV002', N'Tự học Machine Learning', N'CaNhan', N'ND002', NULL);

-- Chi tiết công việc
INSERT INTO ChiTietCongViec (CongViecID, MoTa, NgayBatDau, NgayKetThuc) VALUES 
(N'CV001', N'Thiết kế giao diện frontend với HTML/CSS', '2025-07-02', '2025-07-15'),
(N'CV002', N'Tự học lý thuyết cơ bản về ML và lập trình Python', '2025-07-01', '2025-07-31');

-- Phân công công việc
INSERT INTO PhanCongCongViec (CongViecID, NguoiDungID) VALUES 
(N'CV001', N'ND001'),
(N'CV001', N'ND002');

-- Tiến độ công việc
INSERT INTO TienDoCongViec (CongViecID, PhanTramHoanThanh, SoNgayDaLam) VALUES 
(N'CV001', 50, 7),
(N'CV002', 30, 5);

-- Tiến độ thành viên
INSERT INTO TienDoThanhVien (CongViecID, NguoiDungID, PhanTramHoanThanh, SoNgayDaLam) VALUES 
(N'CV001', N'ND001', 60, 4),
(N'CV001', N'ND002', 40, 3);

-- Hình ảnh
INSERT INTO HinhAnh (ID, DuongDan, LoaiDoiTuong, DoiTuongID) VALUES 
(N'HA001', N'images/users/ND001.jpg', N'NguoiDung', N'ND001'),
(N'HA002', N'images/congviec/CV001.png', N'CongViec', N'CV001');



-- 1. VaiTroNguoiDung
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'VaiTroNguoiDung' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk1 NVARCHAR(200)
    SELECT @pk1 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('VaiTroNguoiDung')
    EXEC('ALTER TABLE VaiTroNguoiDung DROP CONSTRAINT ' + @pk1)
END

ALTER TABLE VaiTroNguoiDung ADD ID nvarchar(12);
UPDATE VaiTroNguoiDung SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE VaiTroNguoiDung ADD CONSTRAINT PK_VaiTroNguoiDung PRIMARY KEY (ID);

ALTER TABLE VaiTroNguoiDung ALTER COLUMN ID nvarchar(12) NOT NULL;


-- 2. TienDoCongViec
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'TienDoCongViec' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk2 NVARCHAR(200)
    SELECT @pk2 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('TienDoCongViec')
    EXEC('ALTER TABLE TienDoCongViec DROP CONSTRAINT ' + @pk2)
END

ALTER TABLE TienDoCongViec ADD ID nvarchar(12);
UPDATE TienDoCongViec SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE TienDoCongViec ADD CONSTRAINT PK_TienDoCongViec PRIMARY KEY (ID);
ALTER TABLE TienDoCongViec ALTER COLUMN ID nvarchar(12) NOT NULL;


-- 3. TienDoThanhVien
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'TienDoThanhVien' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk3 NVARCHAR(200)
    SELECT @pk3 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('TienDoThanhVien')
    EXEC('ALTER TABLE TienDoThanhVien DROP CONSTRAINT ' + @pk3)
END

ALTER TABLE TienDoThanhVien ADD ID nvarchar(12);
UPDATE TienDoThanhVien SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE TienDoThanhVien ADD CONSTRAINT PK_TienDoThanhVien PRIMARY KEY (ID);
ALTER TABLE TienDoThanhVien ALTER COLUMN ID nvarchar(12) NOT NULL;



-- 4. ChiTietCongViec
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'ChiTietCongViec' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk4 NVARCHAR(200)
    SELECT @pk4 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('ChiTietCongViec')
    EXEC('ALTER TABLE ChiTietCongViec DROP CONSTRAINT ' + @pk4)
END

ALTER TABLE ChiTietCongViec ADD ID nvarchar(12);
UPDATE ChiTietCongViec SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE ChiTietCongViec ADD CONSTRAINT PK_ChiTietCongViec PRIMARY KEY (ID);
ALTER TABLE ChiTietCongViec ALTER COLUMN ID nvarchar(12) NOT NULL;


-- 5. PhanCongCongViec
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'PhanCongCongViec' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk5 NVARCHAR(200)
    SELECT @pk5 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('PhanCongCongViec')
    EXEC('ALTER TABLE PhanCongCongViec DROP CONSTRAINT ' + @pk5)
END

ALTER TABLE PhanCongCongViec ADD ID nvarchar(12);
UPDATE PhanCongCongViec SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE PhanCongCongViec ADD CONSTRAINT PK_PhanCongCongViec PRIMARY KEY (ID);
ALTER TABLE PhanCongCongViec ALTER COLUMN ID nvarchar(12) NOT NULL;


-- 6. ThanhVienNhom
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE TABLE_NAME = 'ThanhVienNhom' AND CONSTRAINT_TYPE = 'PRIMARY KEY'
)
BEGIN
    DECLARE @pk6 NVARCHAR(200)
    SELECT @pk6 = name FROM sys.key_constraints WHERE [type] = 'PK' AND parent_object_id = OBJECT_ID('ThanhVienNhom')
    EXEC('ALTER TABLE ThanhVienNhom DROP CONSTRAINT ' + @pk6)
END

ALTER TABLE ThanhVienNhom ADD ID nvarchar(12) ;
UPDATE ThanhVienNhom SET ID = LEFT(REPLACE(NEWID(), '-', ''), 12);
ALTER TABLE ThanhVienNhom ADD CONSTRAINT PK_ThanhVienNhom PRIMARY KEY (ID);
ALTER TABLE ThanhVienNhom ALTER COLUMN ID nvarchar(12) NOT NULL;

EXEC sp_rename 'NguoiDung', 'NguoiDungs';
EXEC sp_rename 'ChiTietCongViec', 'ChiTietCongViecs';
EXEC sp_rename 'CongViec', 'CongViecs';
EXEC sp_rename 'HinhAnh', 'HinhAnhs';
EXEC sp_rename 'NhomCongViec', 'NhomCongViecs';
EXEC sp_rename 'PhanCongCongViec', 'PhanCongCongViecs';
EXEC sp_rename 'ThanhVienNhom', 'ThanhVienNhoms';
EXEC sp_rename 'TienDoCongViec', 'TienDoCongViecs';
EXEC sp_rename 'TienDoThanhVien', 'TienDoThanhViens';
EXEC sp_rename 'VaiTro', 'VaiTros';
EXEC sp_rename 'VaiTroNguoiDung', 'VaiTroNguoiDungs';


ALTER TABLE ThanhVienNhoms
ADD TrangThai NVARCHAR(20) NOT NULL DEFAULT 'ChoDuyet';

ALTER TABLE ThanhVienNhoms
ADD NgayThamGia DATETIME NULL DEFAULT GETDATE();
