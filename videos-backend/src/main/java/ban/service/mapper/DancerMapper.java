package ban.service.mapper;

import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;

import ban.model.persistence.DancerD;
import ban.model.view.Dancer;

/**
 * This is TEMPORARY till I figure out how I want to map between domain models
 * http://kenblair.net/orika-spring-easy-bean-mapping/
 */
@Component
public class DancerMapper {

  public List<Dancer> mapToViewModel(List<DancerD> dancerDs) {

    List<Dancer> dancers = new ArrayList<>();
    for(DancerD dancerD : dancerDs) {
      dancers.add(mapToViewModel(dancerD));
    }

    return dancers;
  }

  public Dancer mapToViewModel(DancerD dancerD) {

    if(dancerD == null) {
      return null;
    }

    Dancer dancer = new Dancer();
    dancer.setWsdcId(dancerD.getWsdcId());
    dancer.setName(dancerD.getName());
    dancer.setVideoIdList(dancerD.getVideoIdList());
    return dancer;
  }

  public DancerD mapToPersistenceModel(Dancer dancer) {

    if(dancer == null) {
      return null;
    }

    DancerD dancerD = new DancerD();
    dancerD.setWsdcId(dancer.getWsdcId());
    dancerD.setName(dancer.getName());
    dancerD.setVideoIdList(dancer.getVideoIdList());
    return  dancerD;
  }
}
