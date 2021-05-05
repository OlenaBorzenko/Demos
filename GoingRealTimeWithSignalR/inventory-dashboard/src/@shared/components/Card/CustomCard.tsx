import React from 'react';
import { Card, CardContent, Collapse, IconButton, Typography } from '@material-ui/core';
import { CardConfig } from '@shared/components/Card/CardConfig';
import { observer } from 'mobx-react';
import clsx from 'clsx';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { useStyles } from '@shared/components/Card/Card.style';
import { IArticleCheckpointModel } from 'Home/store/ArticleCheckpointModel';
import { NestedCard } from '@shared/components/NestedCard';
import ChangeHighlight from 'react-change-highlight';

interface Props {
  cardConfig: CardConfig[];
  item: any;
}

export const CustomCard: React.FC<Props> = observer(props => {
  const { item, cardConfig } = props;
  const s = useStyles();

  return (
    <Card className={s.customCard}>
      <CardContent className={s.cardContent}>
        <Typography
          variant={'h6'}
          className={s.cardHeader}
          color="textPrimary" gutterBottom>
          {item.title}
        </Typography>
        <IconButton
          className={clsx(s.expand, {
            [s.expandOpen]: item.expanded,
          })}
          onClick={item.handleExpand}
          aria-expanded={item.expanded}
          aria-label="show more"
        >
          <ExpandMoreIcon />
        </IconButton>
      </CardContent>
      <Collapse in={item.expanded} timeout="auto" unmountOnExit>
        <CardContent className={s.cardContent}>
          {item.checkpoints.map((checkpoint: IArticleCheckpointModel, i: number) => (
            <ChangeHighlight key={`${name}-${i}`}>
              <div ref={React.createRef()}>
                <NestedCard
                  cardConfig={cardConfig}
                  item={checkpoint} />
              </div>
            </ChangeHighlight>
          ))}
        </CardContent>
      </Collapse>
    </Card>
  );
});
