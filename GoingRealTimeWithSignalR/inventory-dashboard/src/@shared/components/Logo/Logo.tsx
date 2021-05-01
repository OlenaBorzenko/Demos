import React from 'react';
import { Link } from 'react-router-dom';
import { useStyles } from './Logo.style';
import logoImage from './logo-subline.png';
import { Typography } from '@material-ui/core';

export const Logo: React.FC = () => {
  const s = useStyles();

  return (
    <Link to="/" className={s.link}>
      <div className={s.logo}>
        <img className={s.logoIcon} src={logoImage} />
        <Typography variant="h5" color="inherit" className={s.subLogo}>
          APP LOGO
        </Typography>
      </div>
    </Link>
  );
};

Logo.displayName = 'Logo';
