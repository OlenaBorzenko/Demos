import { Button } from '@material-ui/core';
import React from 'react';
import { useStyles } from './FormButtons.style';

export interface FormButtonsProps {
  confirmButtonName: string;
  onReject: () => void;
  onConfirm: () => void;
  rejectButtonName: string;
}

export const FormButtons: React.FC<FormButtonsProps> = props => {
  const s = useStyles();

  const {
    rejectButtonName,
    onReject,
    onConfirm,
    confirmButtonName,
  } = props;

  return (
    <>
      <Button
        className={s.save}
        onClick={onConfirm}
      >
        {confirmButtonName}
      </Button>
      <Button
        className={s.cancel}
        onClick={onReject}
      >
        {rejectButtonName}
      </Button>
    </>
  );
};
