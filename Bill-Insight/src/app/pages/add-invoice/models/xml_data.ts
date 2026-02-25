export interface TTKhac {
  t_truong: string;
  kd_lieu: string;
  d_lieu: string;
}

export interface NBan {
  ten: string;
  mst: string;
  d_chi: string;
  sdt: string;
  tt_khac: TTKhac[];
}

export interface HHDVu {
  id: string;
  mhhdvu: string;
  thhdvu: string;
  dv_tinh: string;
  s_luong: number;
  d_gia: number;
  th_tien: number;
  t_suat: string;
  th_tien_sau_lai_suat: number;
  cash: string;
}

export interface LTSuat {
  t_suat: string;
  th_tien: number;
  t_thue: number;
}

export interface TToan {
  t_httl_t_suat: LTSuat[];
  tg_tc_thue: number;
  tg_t_thue: number;
  tg_tt_tb_so: number;
  tg_tt_tb_chu: string;
  tt_khac: TTKhac[];
}

export interface ReadXmlDataResult {
  nban: NBan;
  hhdvus: HHDVu[];
  ttoan: TToan;
}
