package ban.service;

import org.springframework.stereotype.Component;

import ban.model.persistence.DancerD;
import ban.model.view.Dancer;

/**
 * This is TEMPORARY till I figure out how I want to map between domain models
 * http://kenblair.net/orika-spring-easy-bean-mapping/
 */
@Component
public class DancerMapper {

  public Dancer mapToViewModel(DancerD dancerD) {
    Dancer dancer = new Dancer();
    dancer.setWsdcId(dancerD.getWsdcId());
    dancer.setName(dancerD.getName());
    dancer.setVideoIdList(dancerD.getVideoIdList());
    return dancer;
  }

  public DancerD mapToPersistenceModel(Dancer dancer) {
    DancerD dancerD = new DancerD();
    dancerD.setWsdcId(dancer.getWsdcId());
    dancerD.setName(dancer.getName());
    dancerD.setVideoIdList(dancer.getVideoIdList());
    return  dancerD;
  }
}
